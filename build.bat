@echo off
title NNKZipper Builder (.NET 10)
echo ========================================================
echo Building NNKZipper Windows Application (.NET 10)...
echo ========================================================

:: Check if dotnet CLI is available
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] 'dotnet' command not found! Please ensure .NET SDK is installed.
    pause
    exit /b 1
)

echo [INFO] Building Release configuration...
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================================
    echo SUCCESS! NNKZipper has been successfully built!
    echo ========================================================
    echo Your standalone executable is located in:
    echo   bin\Release\net10.0-windows\win-x64\publish\NNKZipper.exe
    echo.
    echo Remember to place '7zr.exe' in that same folder before running!
    echo.
) else (
    echo [ERROR] Compilation failed.
)

pause