@echo off
title NNKZipper Context Menu Fixer
echo ========================================================
echo Updating NNKZipper Context Menu integration...
echo ========================================================

set "EXE_PATH=C:\Users\KHAN\AppData\Roaming\Cloudbox\NNKZipper\Basic\NNKZipper.exe"

:: Clean old keys
reg delete "HKCU\Software\Classes\Directory\shell\NNKCompress" /f >nul 2>&1
reg delete "HKCU\Software\Classes\*\shell\NNKCompress" /f >nul 2>&1

:: Add direct single-click context menu for Folders
reg add "HKCU\Software\Classes\Directory\shell\NNKCompress" /ve /d "Compress with NNKZipper" /f >nul
reg add "HKCU\Software\Classes\Directory\shell\NNKCompress" /v "Icon" /d "%EXE_PATH%,0" /f >nul
reg add "HKCU\Software\Classes\Directory\shell\NNKCompress\command" /ve /d "\"%EXE_PATH%\" -compress \"%%1\"" /f >nul

:: Add direct single-click context menu for Files
reg add "HKCU\Software\Classes\*\shell\NNKCompress" /ve /d "Compress with NNKZipper" /f >nul
reg add "HKCU\Software\Classes\*\shell\NNKCompress" /v "Icon" /d "%EXE_PATH%,0" /f >nul
reg add "HKCU\Software\Classes\*\shell\NNKCompress\command" /ve /d "\"%EXE_PATH%\" -compress \"%%1\"" /f >nul

ie4uinit.exe -show >nul 2>&1

echo.
echo ========================================================
echo SUCCESS! Right-click menu updated and fully functional!
echo ========================================================
exit