setlocal

@rem Initialize build environment of Visual Studio 2017 or 2019 Community/Professional/Enterprise
@set tools=
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@if not defined tools goto :error
call %tools%
@echo on

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
msbuild ..\src\TimeStampClientGui.MacOs\TimeStampClientGui.MacOs.csproj /p:Configuration=Release /target:Restore || goto :error
msbuild ..\src\TimeStampClientGui.MacOs\TimeStampClientGui.MacOs.csproj /p:Configuration=Release /target:Clean || goto :error
msbuild ..\src\TimeStampClientGui.MacOs\TimeStampClientGui.MacOs.csproj /p:Configuration=Release /target:Build || goto :error

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
copy license\DotNetZip.LICENSE.txt %LICDIR% || goto :error
copy license\DotNetZip.NOTICE.txt %LICDIR% || goto :error
copy license\Eto.Forms.txt %LICDIR% || goto :error
copy license\Eto.Platform.Mac64.txt %LICDIR% || goto :error
copy license\Portable.BouncyCastle.txt %LICDIR% || goto :error
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
