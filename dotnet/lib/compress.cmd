%~dp0/../tools/Bootstrapper.exe -p *spreads_native.dll  ./w32
%~dp0/../tools/Bootstrapper.exe -p *spreads_native.dll  ./w64
%~dp0/../tools/Bootstrapper.exe -p *spreads_native.so  ./l64

move /Y "%~dp0l64\libspreads_native.so.compressed" "%~dp0l64\libspreads_native.compressed"

pause