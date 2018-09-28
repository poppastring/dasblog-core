@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # see DetectChanges.cmd for an explanation of the funny stuff with bash, ping etc. that's going on here
rem #
rem # prints out the message associated with the stash so that the caller can verify it.
rem # $1 = script exit linger time
rem # $2 = the hash of a stash
if [%1] == [] goto error_exit
if [%2] == [] goto error_exit
git log --format=%%B -n 1 %2
set exitcode=%errorlevel%
ping 192.168.0.255 -n 1 -w %1 >NUL
exit %exitcode%
:error_exit
echo one or more command line arguments are missing 1>&2
ping 192.168.0.255 -n 1 -w %1 >NUL
exit 1