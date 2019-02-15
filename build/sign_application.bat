setlocal

set SIGNTOOL="C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe"
set CERTHASH=84babdf3ba22669463db1ccfa7b1c917462bee4a
set TSAURL=http://time.certum.pl/
set LIBNAME=TimeStampClient
set LIBURL=https://github.com/disig/TimeStampClient
set APPDIR=application
set DISIGFILES=%APPDIR%\TimeStampClient.dll %APPDIR%\TimeStampClientGui.exe %APPDIR%\TimeStampClientCmd.exe

%SIGNTOOL% sign /sha1 %CERTHASH% /fd sha1 /tr %TSAURL% /td sha1 /d %LIBNAME% /du %LIBURL% %DISIGFILES% || goto :error
%SIGNTOOL% sign /as /sha1 %CERTHASH% /fd sha256 /tr %TSAURL% /td sha256 /d %LIBNAME% /du %LIBURL% %DISIGFILES% || goto :error

@echo *** SIGNING SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** SIGNING FAILED ***
@endlocal
@exit /b 1