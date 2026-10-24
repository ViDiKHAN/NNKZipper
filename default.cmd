
@echo off
setlocal

set "APP=%AppData%\Cloudbox\NNKZipper\Basic\NNKZipper.exe"

if not exist "%APP%" (
    echo ERROR: NNKZipper.exe was not found:
    echo "%APP%"
    exit /b 1
)

echo Registering NNKZipper for .nnk files...

reg add "HKCU\Software\Classes\.nnk" /ve /d "NNKZipper.NNKFile" /f
if errorlevel 1 goto error

reg add "HKCU\Software\Classes\NNKZipper.NNKFile" /ve /d "NNKZipper NNK File" /f
if errorlevel 1 goto error

reg add "HKCU\Software\Classes\NNKZipper.NNKFile\DefaultIcon" /ve /d "\"%APP%\",0" /f
if errorlevel 1 goto error

reg add "HKCU\Software\Classes\NNKZipper.NNKFile\shell\open\command" /ve /d "\"%APP%\" \"%%1\"" /f
if errorlevel 1 goto error

echo.
echo SUCCESS: .nnk files are registered for NNKZipper.
echo.
echo If Windows has another default app selected, choose NNKZipper
echo in Settings ^> Apps ^> Default apps ^> Choose defaults by file type.
echo.
exit /b 0

:error
echo.
echo ERROR: Could not complete the file association.
exit /b 1
