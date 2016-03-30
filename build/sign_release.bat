setlocal

@rem Initialize build environment of Visual Studio 2015
call "c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat" || exit /b 1

set CERTHASH=84babdf3ba22669463db1ccfa7b1c917462bee4a
set OUTDIR=TimeStampClient-Release
set DISIGFILES=%OUTDIR%\TimeStampClient.dll %OUTDIR%\TimeStampClientGui.exe %OUTDIR%\TimeStampClientCmd.exe
set EXTERNFILES=%OUTDIR%\crypto.dll %OUTDIR%\Eto.Wpf.dll %OUTDIR%\Eto.WinForms.dll %OUTDIR%\MonoMac.dll %OUTDIR%\Eto.Mac.dll %OUTDIR%\Eto.Gtk2.dll %OUTDIR%\Eto.Gtk3.dll %OUTDIR%\Eto.dll %OUTDIR%\Ionic.Zip.Reduced.dll

signtool sign /sha1 %CERTHASH% /fd sha1 /tr http://time.certum.pl/ /td sha1 /d "TimeStampClient" /du "https://github.com/disig/TimeStampClient" %DISIGFILES% || exit /b 1
signtool sign /as /sha1 %CERTHASH% /fd sha256 /tr http://time.certum.pl/ /td sha256 /d "TimeStampClient" /du "https://github.com/disig/TimeStampClient" %DISIGFILES% || exit /b 1

signtool sign /sha1 %CERTHASH% /fd sha1 /tr http://time.certum.pl/ /td sha1 %EXTERNFILES% || exit /b 1
signtool sign /as /sha1 %CERTHASH% /fd sha256 /tr http://time.certum.pl/ /td sha256 %EXTERNFILES% || exit /b 1

endlocal

@echo *** SIGNING SUCCESSFUL ***
@exit /b 0
