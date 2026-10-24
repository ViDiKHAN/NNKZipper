@echo off
title NNKZipper De-association & Cleanup
echo ========================================================
echo Removing NNKZipper registry entries and file associations...
echo ========================================================

:: 1. Remove .nnk file association
reg delete "HKCU\Software\Classes\.nnk" /f >nul 2>&1

:: 2. Remove NNKArchive file type definition & icons
reg delete "HKCU\Software\Classes\NNKArchive" /f >nul 2>&1

:: 3. Remove Directory (folder) right-click context menu item
reg delete "HKCU\Software\Classes\Directory\shell\NNKCompress" /f >nul 2>&1

:: 4. Remove File right-click context menu item
reg delete "HKCU\Software\Classes\*\shell\NNKCompress" /f >nul 2>&1

:: 5. Refresh Windows Explorer shell cache to apply changes immediately
ie4uinit.exe -show >nul 2>&1

echo.
echo ========================================================
echo SUCCESS! All NNKZipper file associations and context 
echo menu shortcuts have been completely removed.
echo ========================================================
exit