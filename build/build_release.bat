setlocal

@rem Initialize Visual Studio build environment:
@rem - Visual Studio 2017 Community/Professional/Enterprise is the preferred option
@set tools=
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@if not defined tools goto :error
call %tools%
@echo on

@rem Build for AnyCPU platform
msbuild ..\src\DisigTimeStamp.sln /p:Configuration=Release /p:Platform="Any CPU" /target:Clean || exit /b 1
msbuild ..\src\DisigTimeStamp.sln /p:Configuration=Release /p:Platform="Any CPU" /target:Restore /target:ReBuild || exit /b 1

endlocal

setlocal

@rem Prepare output directory
set OUTDIR=TimeStampClient-Release
set LICDIR=%OUTDIR%\license
rmdir /S /Q %OUTDIR%
mkdir %OUTDIR% || exit /b 1
mkdir %LICDIR% || exit /b 1

@rem Copy binaries to the output directory
copy ..\src\TimeStampClientGui\bin\Release\TimeStampClientGui.exe %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\TimeStampClientGui.exe.config %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\TimeStampClient.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\BouncyCastle.Crypto.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Wpf.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.WinForms.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\MonoMac.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Mac64.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Gtk2.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Gtk3.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Ionic.Zip.Reduced.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientCmd\bin\Release\TimeStampClientCmd.exe %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientCmd\bin\Release\TimeStampClientCmd.exe.config %OUTDIR% || exit /b 1

@rem Copy licenses to the output directory
copy license\*.txt %LICDIR% || exit /b 1

endlocal

@echo *** BUILD SUCCESSFUL ***
@exit /b 0
