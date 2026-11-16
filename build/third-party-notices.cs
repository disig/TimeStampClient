// Generates a single text file with license information of all NuGet packages
// (including transitive ones) used by the given project.
//
// Usage: dotnet run third-party-notices.cs -- <project file> <output file>
//
// The project must be restored first (obj\project.assets.json must exist),
// which is always the case after "dotnet publish".
//
// For every package the following is included:
// - license and copyright from the package .nuspec
// - LICENSE, NOTICE and THIRD-PARTY-NOTICES files shipped in the package (identical files are included only once)
// - full text of the license expression (MIT, ...) taken from LicenseTexts below
//
// The program fails when a package uses a license expression not present in LicenseTexts
// or when a package contains no license information at all, so new licenses never slip in unnoticed.

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: dotnet run third-party-notices.cs -- <project file> <output file>");
    return 1;
}

try
{
    GenerateNotices(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]));
    return 0;
}
catch (LicenseException e)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

static void GenerateNotices(string projectFile, string outputFile)
{
    string assetsFile = Path.Combine(Path.GetDirectoryName(projectFile)!, "obj", "project.assets.json");
    if (!File.Exists(assetsFile))
    {
        throw new LicenseException($"File {assetsFile} not found. Restore the project first.");
    }

    using JsonDocument assets = JsonDocument.Parse(File.ReadAllText(assetsFile));
    // Global packages folder followed by fallback folders (e.g. Visual Studio offline packages)
    string[] packageFolders = assets.RootElement.GetProperty("packageFolders").EnumerateObject().Select(folder => folder.Name).ToArray();

    // Library names have the format "Id/Version"
    IEnumerable<JsonProperty> packages = assets.RootElement.GetProperty("libraries").EnumerateObject()
        .Where(library => library.Value.GetProperty("type").GetString() == "package")
        .OrderBy(library => library.Name.Split('/')[0], StringComparer.OrdinalIgnoreCase);

    string separator = new string('=', 80);
    StringBuilder summary = new StringBuilder();
    StringBuilder sections = new StringBuilder();
    Dictionary<string, string> includedFiles = new Dictionary<string, string>(); // file hash -> description of the already included file
    SortedSet<string> usedExpressions = new SortedSet<string>();
    int packageCount = 0;

    foreach (JsonProperty package in packages)
    {
        string relativePath = package.Value.GetProperty("path").GetString()!;
        string? packageDir = packageFolders
            .Select(folder => Path.GetFullPath(Path.Combine(folder, relativePath)))
            .FirstOrDefault(Directory.Exists);
        if (packageDir == null)
        {
            throw new LicenseException($"Package {package.Name} not found in any of the package folders: {string.Join(", ", packageFolders)}");
        }

        string nuspecFile = Directory.GetFiles(packageDir, "*.nuspec").First();
        XElement metadata = XDocument.Load(nuspecFile).Root!.Elements().First(e => e.Name.LocalName == "metadata");
        string packageName = $"{GetValue(metadata, "id")} {GetValue(metadata, "version")}";

        // License files shipped in the package root
        List<string> files = Directory.GetFiles(packageDir)
            .Where(file => Regex.IsMatch(Path.GetFileName(file), "^(licen[cs]e|notice|third-party-notices)", RegexOptions.IgnoreCase))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        string license = "unknown";
        XElement? licenseElement = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "license");
        string? licenseType = licenseElement?.Attribute("type")?.Value;

        if (licenseType == "expression")
        {
            license = licenseElement!.Value;
            if (!LicenseTexts.All.ContainsKey(license))
            {
                throw new LicenseException($"Package {packageName} uses license {license} which is not known. Review the license and add its text to LicenseTexts in third-party-notices.cs");
            }

            usedExpressions.Add(license);
        }
        else if (licenseType == "file")
        {
            license = $"see {licenseElement!.Value}";
            string licenseFile = Path.GetFullPath(Path.Combine(packageDir, licenseElement.Value));
            if (!files.Contains(licenseFile, StringComparer.OrdinalIgnoreCase))
            {
                files.Add(licenseFile);
            }
        }
        else if (files.Count == 0)
        {
            throw new LicenseException($"Package {packageName} contains no license information. Review it manually.");
        }

        packageCount++;
        summary.AppendLine($"  {packageName,-50} {license}");

        sections.AppendLine(separator);
        sections.AppendLine(packageName);
        sections.AppendLine(separator);
        sections.AppendLine($"License: {license}");

        string? copyright = GetValue(metadata, "copyright");
        if (!string.IsNullOrEmpty(copyright))
        {
            sections.AppendLine($"Copyright: {copyright}");
        }

        string? projectUrl = GetValue(metadata, "projectUrl");
        if (!string.IsNullOrEmpty(projectUrl))
        {
            sections.AppendLine($"Project URL: {projectUrl}");
        }

        sections.AppendLine();

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file)));

            sections.AppendLine($"--- {fileName} ---");
            if (includedFiles.TryGetValue(hash, out string? includedBy))
            {
                sections.AppendLine($"Identical to {includedBy} included above.");
            }
            else
            {
                includedFiles[hash] = $"{fileName} of {packageName}";
                sections.AppendLine(File.ReadAllText(file).TrimEnd());
            }

            sections.AppendLine();
        }
    }

    StringBuilder output = new StringBuilder();
    output.AppendLine("This software uses the following third-party NuGet packages:");
    output.AppendLine();
    output.Append(summary);
    output.AppendLine();
    output.Append(sections);

    foreach (string expression in usedExpressions)
    {
        output.AppendLine(separator);
        output.AppendLine($"{expression} license text (applies to all packages licensed under {expression} with their copyright notices listed above)");
        output.AppendLine(separator);
        output.AppendLine(LicenseTexts.All[expression].ReplaceLineEndings().Trim());
        output.AppendLine();
    }

    File.WriteAllText(outputFile, output.ToString(), new UTF8Encoding(false));
    Console.WriteLine($"Third-party notices for {packageCount} packages written to {outputFile}");
}

static string? GetValue(XElement metadata, string name)
{
    return metadata.Elements().FirstOrDefault(e => e.Name.LocalName == name)?.Value;
}

// Reviewed licenses allowed for packages declaring license expression in .nuspec
static class LicenseTexts
{
    public static readonly Dictionary<string, string> All = new Dictionary<string, string>
    {
        ["MIT"] = """
            MIT License

            Permission is hereby granted, free of charge, to any person obtaining a copy
            of this software and associated documentation files (the "Software"), to deal
            in the Software without restriction, including without limitation the rights
            to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
            copies of the Software, and to permit persons to whom the Software is
            furnished to do so, subject to the following conditions:

            The above copyright notice and this permission notice shall be included in all
            copies or substantial portions of the Software.

            THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
            AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
            LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
            OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
            SOFTWARE.
            """,
    };
}

class LicenseException : Exception
{
    public LicenseException(string message)
        : base(message)
    {
    }
}
