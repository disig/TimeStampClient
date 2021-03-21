setlocal

@rem Remove leftovers of any previous builds
rmdir /S /Q ..\src\TimeStampClient\bin
rmdir /S /Q ..\src\TimeStampClient\obj
rmdir /S /Q ..\src\TimeStampClientCmd\bin
rmdir /S /Q ..\src\TimeStampClientCmd\obj
rmdir /S /Q ..\src\TimeStampClientGui.Linux\bin
rmdir /S /Q ..\src\TimeStampClientGui.Linux\obj
rmdir /S /Q ..\src\TimeStampClientGui.MacOs\bin
rmdir /S /Q ..\src\TimeStampClientGui.MacOs\obj
rmdir /S /Q ..\src\TimeStampClientGui.Windows\bin
rmdir /S /Q ..\src\TimeStampClientGui.Windows\obj

@rem Build release
dotnet publish -c Release ..\src\TimeStampClientGui.MacOs\TimeStampClientGui.MacOs.csproj

@rem Copy binaries to the output directory
set APPDIR=TimeStampClientGui.MacOs.app
rmdir /S /Q %APPDIR%
mkdir %APPDIR% || goto :error
xcopy ..\src\TimeStampClientGui.MacOs\bin\Release\net5.0\TimeStampClientGui.MacOs.app\ %APPDIR% /E  || goto :error
del %APPDIR%\Contents\MacOS\*.pdb
del %APPDIR%\Contents\MacOS\*.xml

@rem Copy licenses to the output directory
set LICDIR=%APPDIR%\Contents\Resources\license
mkdir %LICDIR% || goto :error
copy license\BouncyCastle.Crypto.txt %LICDIR% || goto :error
copy license\DotNetZip.LICENSE.txt %LICDIR% || goto :error
copy license\DotNetZip.NOTICE.txt %LICDIR% || goto :error
copy license\Eto.Forms.txt %LICDIR% || goto :error
copy license\Eto.Platform.Mac64.txt %LICDIR% || goto :error
copy license\TimeStampClient.LICENSE.txt %LICDIR% || goto :error
copy license\TimeStampClient.NOTICE.txt %LICDIR% || goto :error

endlocal

@echo *** BUILD SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** BUILD FAILED ***
@endlocal
@exit /b 1
