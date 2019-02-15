@setlocal

@rem Define paths to necessary directories
set workingdir=%~dp0
set inputdir=%workingdir%nuget
set outputdir=%workingdir%nuget-signed

@rem Define paths to necessary tools
set NUGET=c:\nuget\nuget.exe 
set SEVENZIP="c:\Program Files\7-Zip\7z.exe"
set SIGNTOOL="C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe"

@rem Define signing options
set CERTHASH=84babdf3ba22669463db1ccfa7b1c917462bee4a
set TSAURL=http://time.certum.pl/
set LIBNAME=TimeStampClient
set LIBURL=https://github.com/disig/TimeStampClient

@rem Clean output directory
rmdir /S /Q %outputdir%
mkdir %outputdir% || goto :error

@rem Copy unsigned package to output directory
copy %inputdir%\*.nupkg %outputdir% || goto :error

@rem Extract unsigned package contents into the output directory
cd %outputdir% || goto :error
%SEVENZIP% x *.nupkg || goto :error
rmdir /S /Q _rels || goto :error
rmdir /S /Q package || goto :error
del /Q *.xml || goto :error
del /Q *.nupkg || goto :error

@rem Sign all assemblies using SHA1withRSA algorithm
%SIGNTOOL% sign /sha1 %CERTHASH% /fd sha1 /tr %TSAURL% /td sha1 /d %LIBNAME% /du %LIBURL% ^
lib\net45\TimeStampClient.dll ^
lib\netstandard2.0\TimeStampClient.dll || goto :error

@rem Sign all assemblies using SHA256withRSA algorithm
%SIGNTOOL% sign /sha1 %CERTHASH% /as /fd sha256 /tr %TSAURL% /td sha256 /d %LIBNAME% /du %LIBURL% ^
lib\net45\TimeStampClient.dll ^
lib\netstandard2.0\TimeStampClient.dll || goto :error

@rem Create signed package with signed assemblies
%NUGET% pack TimeStampClient.nuspec || goto :error
%NUGET% sign TimeStampClient*.nupkg -CertificateFingerprint %CERTHASH% -Timestamper %TSAURL% || goto :error
%NUGET% verify -Signature TimeStampClient*.nupkg || goto :error
copy %inputdir%\*.snupkg . || goto :error

@rem Clean up
rmdir /S /Q lib || goto :error
del /Q *.nuspec || goto :error
del /Q *.txt || goto :error

@echo *** SIGN SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** SIGN FAILED ***
@endlocal
@exit /b 1
