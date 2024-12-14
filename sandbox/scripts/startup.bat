REM Download windowsdesktop-runtime
curl -L "https://download.visualstudio.microsoft.com/download/pr/76e5dbb2-6ae3-4629-9a84-527f8feb709c/09002599b32d5d01dc3aa5dcdffcc984/windowsdesktop-runtime-8.0.6-win-x64.exe" --output C:\users\WDAGUtilityAccount\Downloads\windowsdesktop-runtime.exe

REM Install windowsdesktop-runtime
C:\users\WDAGUtilityAccount\Downloads\windowsdesktop-runtime.exe /norestart /quiet

REM Download aspcore-runtime
curl -L "https://download.visualstudio.microsoft.com/download/pr/38b32fc8-8070-4f14-bd52-65505fddc5ff/50e6cf3b7505eee02c3b3db8ea46ffe3/aspnetcore-runtime-8.0.6-win-x64.exe" --output C:\users\WDAGUtilityAccount\Downloads\aspcore-runtime.exe

REM Install aspcore-runtime
C:\users\WDAGUtilityAccount\Downloads\aspcore-runtime.exe /norestart /quiet

REM Install VC Redist
C:\Dev\Vixen\Installer\Redist\VC_redist.x64.exe /install /passive /norestart

REM Launch App install panel
appwiz.cpl

REM open output folder
explorer "c:\Dev\Vixen"