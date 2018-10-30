@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # work around xunit's behaviour of redirecting stdout by spawning a process to deliver a message
rem # $1 = message
rem # usage AlertUser.cmd <message>
rem # e.d. cmd /c Alert.cmd "this is an emergency - don't panic"
color C
echo %0
if [%1] == [] goto err_exit
echo %1
goto eof
rem # timeout /t -1
rem # exit 0
:err_exit
rem #    exit 1
exit /b 1
:eof
exit /b 0