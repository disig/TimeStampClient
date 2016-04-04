# TimeStampClient
Easy to use .NET RFC 3161 time-stamp client library and applications based on [Bouncy Castle library](https://www.bouncycastle.org/).

TimeStampClient:
* runs on Windows, Linux and Mac OS X
* supports Time-Stamp Protocol over HTTP/HTTPS and TCP
* supports SSL authentication (client certificate) and HTTP basic authentication (login and password)
* uses system HTTP proxy settings
* contains GUI application
* contains command line application
* contains reusable .NET library

## Download
[TimeStampClient-1.0.0.zip](https://github.com/disig/TimeStampClient/releases/download/v1.0.0/TimeStampClient-1.0.0.zip) contains command line tool, GUI application and library. Same archive can be used on all supported operating systems (Windows, Linux, Mac OS X).

NuGet package is available at [TimeStampClient.1.0.0.nupkg](https://github.com/disig/TimeStampClient/releases/download/v1.0.0/TimeStampClient.1.0.0.nupkg).

## Usage
### TimeStampClient Library

In most cases you just need the following one line of code to get a time-stamp.
```csharp
using Disig.TimeStampClient;

...

TimeStampToken token = TimeStampClient.RequestTimeStampToken("http://localhost/tsa", "document.docx");

...
```

### TimeStampClient command line application
[Mono runtime](http://www.mono-project.com/) is required to use TimeStampClient applications on Linux and Mac OS X.

Getting time-stamp using the command line application on Windows:
```
> TimeStampClientCmd.exe --tsa http://localhost/tsa --file document.docx --out token.tst
```

Getting time-stamp using the command line application on Linux and Mac OS X:
```
$ mono ./TimeStampClientCmd.exe --tsa http://localhost/tsa --file document.docx --out token.tst
```

### TimeStampClient GUI application
On Windows start the TimeStampClient application by double-clicking on `TimeStampClientGui.exe` file.

On Linux and Mac OS X start the TimeStampClientGui application by running the following command from command line:
```
$ mono ./TimeStampClientGui.exe
```

User needs to specify the URL of a time-stamping authority in the "TSA service URL" field and the path to a file to be time-stamped in the "File to time-stamp" field. After clicking on the "Request time-stamp" button the time-stamp token is saved to the file specified in the "Output file" field.

![TimeStampClient screenshot](doc/images/screenshot-windows.png?raw=true)


## License
TimeStampClient library and applications are available under the terms of the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

## About
TimeStampClient library and applications are provided by [Disig a.s.](https://www.disig.sk/)
