# NNKZipper — Custom Windows File Compression Tool

**NNKZipper** is a lightweight, high-performance Windows desktop application designed to pack and unpack compressed archives using the custom `.nnk` file extension, powered by the battle-tested **7-Zip (`7zr.exe`)** compression engine under the hood.

---

## Features
- **Custom `.nnk` Format:** Standard 7z-compliant architecture wrapped in your own branded extension.
- **Drag & Drop Interface:** Simply drag files and folders directly onto the application list.
- **Compression Level Presets:** From Store (0) up to Ultra Max (9).
- **AES-256 Encryption:** Secure archives with passwords and optional encrypted file headers (`-mhe=on`).
- **Archive Explorer:** Inspect contents of any `.nnk` archive before extracting.
- **Portable Architecture:** Standalone single-directory build (`NNKZipper.exe` + `7zr.exe`).
- **Shell File Association:** One-click registration so double-clicking `.nnk` files opens them directly in your tool.

---

## Quick Setup Guide

1. **Get the Engine (`7zr.exe`):**
   - Download the official standalone 7-Zip command-line version (`7zr.exe`) from the official 7-Zip website or copy it from an existing 7-Zip installation.
   - Place `7zr.exe` in the exact same folder where `NNKZipper.exe` is located.

2. **Compile the App:**
   - Save the provided code files (`Program.cs`, `MainForm.cs`, and `build.bat`) in a single folder.
   - Double-click **`build.bat`**. The script will use the built-in Microsoft C# compiler (`csc.exe`) to generate **`NNKZipper.exe`**.

3. **Run and Enjoy:**
   - Launch `NNKZipper.exe`.
   - Add files or folders, choose your compression settings, and click **Create .nnk Archive**!