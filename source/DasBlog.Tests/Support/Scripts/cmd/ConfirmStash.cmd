@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem #
rem # prints out the message associated with the stash so that the caller can verify it.
rem # $1 = the hash of a stash
echo dasmeta %time% %0 %1
echo dasmeta_output_start
echo dasmeta_errors_start
if [%1] == [] goto error_exit
git log --format=%%B -n 1 %1 2>&1
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%

:error_exit
echo one or more command line arguments are missing
echo dasmeta_errors_complete
exit 1