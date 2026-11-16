@setlocal

@rem Prepare output directory
set APPDIR=nuget-unsigned
rmdir /S /Q %APPDIR%
mkdir %APPDIR% || goto :error

@rem Remove leftovers of any previous builds
rmdir /S /Q ..\src\TimeStampClient\bin
rmdir /S /Q ..\src\TimeStampClient\obj
rmdir /S /Q ..\src\TimeStampClientCmd\bin
rmdir /S /Q ..\src\TimeStampClientCmd\obj
rmdir /S /Q ..\src\TimeStampClientGui\bin
rmdir /S /Q ..\src\TimeStampClientGui\obj

@rem Build and publish nuget package
dotnet pack ..\src\TimeStampClient\TimeStampClient.csproj -c Release -o %APPDIR% || goto :error

@echo *** BUILD SUCCESSFUL ***
@endlocal
@exit /b 0

:error
@echo *** BUILD FAILED ***
@endlocal
@exit /b 1
