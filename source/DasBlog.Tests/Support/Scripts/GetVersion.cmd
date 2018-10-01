@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # see DetectChanges.cmd for an explanation of the funny stuff with ping etc. that's going on here
rem # returns the version of GIT installed
rem # $1 = script exit linger time
if [%1] == [] goto error_exit
git --version
set exitcode=%errorlevel%
ping 192.168.0.255 -n 1 -w %1 >NUL
exit %exitcode%
:error_exit
echo the script exit linger time was omitted 1>&2
ping 192.168.0.255 -n 1 -w %1 >NUL
exit 1