@setlocal

@rem Prepare output directory
set APPDIR=TimeStampClientGui.Linux
rmdir /S /Q %APPDIR%
mkdir %APPDIR% || goto :error

@rem Remove leftovers of any previous builds
rmdir /S /Q ..\src\TimeStampClient\bin
rmdir /S /Q ..\src\TimeStampClient\obj
rmdir /S /Q ..\src\TimeStampClientCmd\bin
rmdir /S /Q ..\src\TimeStampClientCmd\obj
rmdir /S /Q ..\src\TimeStampClientGui\bin
rmdir /S /Q ..\src\TimeStampClientGui\obj

@rem Build and publish release
dotnet publish ..\src\TimeStampClientGui\TimeStampClientGui.csproj -c Release -r linux-x64 --self-contained false -o %APPDIR% || goto :error

@rem Remove unnecessary items
del %APPDIR%\*.pdb
del %APPDIR%\*.xml

@rem Copy licenses to the output directory
set LICDIR=%APPDIR%\license
mkdir %LICDIR% || goto :error
copy license\Avalonia.LICENSE.md %LICDIR% || goto :error
copy license\Avalonia.NOTICE.md %LICDIR% || goto :error
copy license\BouncyCastle.LICENSE.md %LICDIR% || goto :error
copy license\SharpZipLib.LICENSE.txt %LICDIR% || goto :error
copy license\TimeStampClient.LICENSE.txt %LICDIR% || goto :error
copy license\TimeStampClient.NOTICE.txt %LICDIR% || goto :error

@rem Generate THIRD-PARTY-NOTICES.txt with licenses of all NuGet packages used by the application
dotnet run third-party-notices.cs -- ..\src\TimeStampClientGui\TimeStampClientGui.csproj %LICDIR%\THIRD-PARTY-NOTICES.txt || goto :error

@echo *** BUILD SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** BUILD FAILED ***
@endlocal
@exit /b 1
