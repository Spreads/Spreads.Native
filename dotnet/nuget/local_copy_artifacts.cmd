@echo off

dir ..\artifacts

setlocal
:PROMPT
SET /P AREYOUSURE=Copy to local? (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END

xcopy /s ..\artifacts \transient\LocalNuget

:END
endlocal

pause