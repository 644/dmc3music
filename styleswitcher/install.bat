@echo off

cd /d "%~dp0"

md "_Restore"

for /f "delims=" %%a in ('dir /A /B') do (
	if not "%%a"=="_Restore" (
	if not "%%a"=="data" (
	if not "%%a"=="data2" (
	if not "%%a"=="mov" (
	if not "%%a"=="sound" (
	if not "%%a"=="dinput8.dll" (
	if not "%%a"=="dmc3se.exe" (
	if not "%%a"=="input.dat" (
	if not "%%a"=="install.bat" (
	if not "%%a"=="save0.sav" (
	if not "%%a"=="snd.drv" (
	if not "%%a"=="StyleSwitcher.dll" (
	if not "%%a"=="StyleSwitcher.ini" (
	if not "%%a"=="uninstall.bat" (
		move "%%a" "_Restore\%%a"
	)
	)
	)
	)
	)
	)
	)
	)
	)
	)
	)
	)
	)
	)
)

move "data\GData.pak" "GDATA.AFS"
move "data" "_Restore\data"

md "native"

move "data2" "native\rom"
move "mov" "native\movie"
move "sound" "native\sound"

ren "native\movie\ipu\*.bin" "*.bmp"

ren "native\sound\*.bin" "*.ogg"

del "install.bat"
