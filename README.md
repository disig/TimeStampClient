# TimeStampClient
Easy to use .NET RFC 3161 time-stamp client library and applications based on [Bouncy Castle library](https://www.bouncycastle.org/).

TimeStampClient:
* runs on Windows, Linux and macOS
* supports Time-Stamp Protocol over HTTP/HTTPS and TCP
* supports SSL authentication (client certificate) and HTTP basic authentication (login and password)
* uses system HTTP proxy settings
* contains GUI application
* contains command line application
* contains reusable .NET library

## Download

There are multiple [release artifacts](https://github.com/disig/TimeStampClient/releases/latest) available:

* `TimeStampClient.nupkg` - NuGet package with reusable .NET library
* `TimeStampClient.snupkg` - NuGet package with debug symbols for reusable .NET library
* `TimeStampClientCmd.zip` - Command line tool usable on Windows, Linux and macOS
* `TimeStampClientGui.Linux.zip` - GUI application usable on Linux
* `TimeStampClientGui.MacOs.zip` - GUI application usable on macOS
* `TimeStampClientGui.Windows.zip` - GUI application usable on Windows

Official [NuGet package](https://www.nuget.org/packages/TimeStampClient/) is published in nuget.org repository.

## Usage
### TimeStampClient Library

In most cases you just need the following single line of code to get a time-stamp.
```csharp
var timeStampToken = Disig.TimeStampClient.TimeStampClient.RequestTimeStampToken("http://localhost/tsa", "document.docx");
```

### TimeStampClient command line application
[.NET 5.0 runtime](https://dotnet.microsoft.com/download/dotnet/5.0) is required on all platforms in order to use `TimeStampClientCmd` application.

Getting time-stamp using the command line application on Windows:
```
> TimeStampClientCmd.exe --tsa http://localhost/tsa --file document.docx --out token.tst
```

Getting time-stamp using the command line application on Linux and macOS:
```
$ dotnet ./TimeStampClientCmd.dll --tsa http://localhost/tsa --file document.docx --out token.tst
```

### TimeStampClient GUI application

On Windows:
- Extract `TimeStampClientGui.Windows.zip` archive
- Start the application by double-clicking on `TimeStampClientGui.exe` file

On Linux:
- Install [.NET 5.0 runtime](https://dotnet.microsoft.com/download/dotnet/5.0)
- Extract `TimeStampClientGui.Linux.zip` archive
- Start the application with the following command:
  ```
  $ dotnet ./TimeStampClientGui.Linux.dll
  ```

On macOS:
- Install [.NET 5.0 runtime](https://dotnet.microsoft.com/download/dotnet/5.0)
- Extract `TimeStampClientGui.MacOs.zip` archive
- Execute following commands from command line:
  ```sh
  $ chmod +x TimeStampClientGui.MacOs.app/Contents/MacOS/TimeStampClientGui.MacOs
  $ xattr -c TimeStampClientGui.MacOs.app
  ```
- Start the application by double-clicking on its icon

User needs to specify the URL of a time-stamping authority in the "TSA service URL" field and the path to a file to be time-stamped in the "File to time-stamp" field. After clicking on the "Request time-stamp" button the time-stamp token is saved to the file specified in the "Output file" field.

![TimeStampClient screenshot](doc/images/screenshot-windows.png?raw=true)

## License
TimeStampClient library and applications are available under the terms of the [Apache License, Version 2.0](https://www.apache.org/licenses/LICENSE-2.0).  
[Human friendly license summary](https://tldrlegal.com/l/apache2) is available at tldrlegal.com but the [full license text](LICENSE.txt) always prevails.

## About
TimeStampClient library and applications are provided by [Disig a.s.](https://www.disig.sk/)
