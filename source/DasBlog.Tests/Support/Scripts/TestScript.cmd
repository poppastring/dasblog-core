@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
echo testing 1 2 3 4
echo abc
echo this is a mistake 1>&2
rem # if we don't have something other than "echo" then the output is not caught by the caller
set
set exitcode=%errorlevel%
ping 168.192.0.255 -n 1 -w %1
exit %exitcode%
