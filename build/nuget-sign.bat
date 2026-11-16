@setlocal

@rem Define paths to necessary directories
set workingdir=%~dp0
set inputdir=%workingdir%nuget-unsigned
set outputdir=%workingdir%nuget-signed

@rem Define paths to necessary tools
set NUGET=c:\nuget\nuget.exe 
set SEVENZIP="c:\Program Files\7-Zip\7z.exe"
set SIGNTOOL="C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe"

@rem Define signing options
set CERTHASHSHA1=5A640AD3AD0DE8608BB38C7686258A2440D3C3C2
set CERTHASHSHA256=183DF782842BE1370C6559B98D6F3C0C9D18556BA93811C0A31D7090923AFAF7
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

@rem Sign all assemblies using SHA256withRSA algorithm
%SIGNTOOL% sign /sha1 %CERTHASHSHA1% /fd sha256 /tr %TSAURL% /td sha256 /d %LIBNAME% /du %LIBURL% ^
lib\net462\TimeStampClient.dll ^
lib\netstandard2.0\TimeStampClient.dll || goto :error

@rem Create signed package with signed assemblies
%NUGET% pack TimeStampClient.nuspec || goto :error
%NUGET% sign TimeStampClient*.nupkg -CertificateFingerprint %CERTHASHSHA256% -Timestamper %TSAURL% || goto :error
%NUGET% verify -Signature TimeStampClient*.nupkg || goto :error
copy %inputdir%\*.snupkg . || goto :error

@rem Clean up
rmdir /S /Q lib || goto :error
del /Q *.nuspec || goto :error
del /Q *.png || goto :error
del /Q *.txt || goto :error

@echo *** SIGN SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** SIGN FAILED ***
@endlocal
@exit /b 1
