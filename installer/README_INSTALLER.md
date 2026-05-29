Steps to create a single installer .exe for SchoolDeviceStore

1. Build and publish the app (on your dev machine with Visual Studio/MSBuild):

```powershell
cd "d:\1_Tran Van Long\NCKH\LTNC"
.\scripts\publish-windows-gui.ps1
```

This will create a `publish` folder next to the repo root containing the exe and all required files.

2. Install Inno Setup (https://jrsoftware.org) on the same dev machine.

3. Edit `installer/SchoolDeviceStore.iss` if your executable name differs from `SchoolDeviceStore.GUI.exe`:
- Open the `.iss` file and change the `MyAppExe` line.

4. Compile the installer:
- Either open `SchoolDeviceStore.iss` in Inno Setup UI and click 'Compile',
- Or run from command line (path to ISCC.exe may vary):

```powershell
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\SchoolDeviceStore.iss
```

5. Result:
- The installer `SchoolDeviceStore-Setup.exe` will be generated in the default Inno output folder.
- Copy that `.exe` to any target machine and run it. It will install the app into `Program Files` and create shortcuts.

6. On target machines:
- If you see errors about missing native modules after install, install the Visual C++ Redistributable (2015-2022 x64).

If you want, I can now:
- Generate the `.iss` (done) and a zipped instructions file for you to download; or
- Try to compile the installer here (not possible because this environment lacks MSBuild/Inno). 

Which next step do you want me to take?