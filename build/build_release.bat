setlocal

@rem Initialize build environment of Visual Studio 2015
call "c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"

@rem Build for AnyCPU platform
msbuild ..\src\DisigTimeStamp.sln /p:Configuration=Release /p:Platform="Any CPU" /target:Clean || exit /b 1
msbuild ..\src\DisigTimeStamp.sln /p:Configuration=Release /p:Platform="Any CPU" /target:Build || exit /b 1

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
copy ..\src\TimeStampClientGui\bin\Release\crypto.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Wpf.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.WinForms.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\MonoMac.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Mac.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Gtk3.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.Gtk2.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Eto.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientGui\bin\Release\Ionic.Zip.Reduced.dll %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientCmd\bin\Release\TimeStampClientCmd.exe %OUTDIR% || exit /b 1
copy ..\src\TimeStampClientCmd\bin\Release\TimeStampClientCmd.exe.config %OUTDIR% || exit /b 1

@rem Copy licenses to the output directory
copy ..\src\lib\DotNetZip.Reduced\LICENSE.txt %LICDIR%\DotNetZip.Reduced.txt || exit /b 1
copy ..\src\lib\Eto.Forms\LICENSE.txt %LICDIR%\Eto.Forms.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Gtk\LICENSE.txt %LICDIR%\Eto.Platform.Gtk.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Gtk3\LICENSE.txt %LICDIR%\Eto.Platform.Gtk3.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Mac\LICENSE.txt %LICDIR%\Eto.Platform.Mac.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Mac\MonoMac-License.txt %LICDIR%\MonoMac.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Windows\LICENSE.txt %LICDIR%\Eto.Platform.Windows.txt || exit /b 1
copy ..\src\lib\Eto.Platform.Wpf\LICENSE.txt %LICDIR%\Eto.Platform.Wpf.txt || exit /b 1
copy ..\LICENSE.txt %LICDIR%\LICENSE.txt || exit /b 1
copy ..\NOTICE.txt %LICDIR%\NOTICE.txt || exit /b 1
endlocal

@echo *** BUILD SUCCESSFUL ***
@exit /b 0
