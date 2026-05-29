Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap 256, 256
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Transparent)
$g.FillEllipse([System.Drawing.Brushes]::DodgerBlue, 16, 16, 224, 224)
$g.FillEllipse([System.Drawing.Brushes]::White, 64, 64, 128, 128)

$iconPath = "d:\1_Tran Van Long\NCKH\LTNC\SchoolDeviceStore\GUI.WinForms\app.ico"
$fs = New-Object System.IO.FileStream $iconPath, 'Create'

$hIcon = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)
$icon.Save($fs)

$fs.Close()
$icon.Dispose()
$g.Dispose()
$bmp.Dispose()
