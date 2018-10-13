@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
echo dasmeta %time% %0
echo dasmeta_output_start
echo dasmeta_errors_start
echo testing 1 2 3 4
echo abc
echo this is a mistake
rem # if we don't have something other than "echo" then the output is not caught by the caller
set
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%
