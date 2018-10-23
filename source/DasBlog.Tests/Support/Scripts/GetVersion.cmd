@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # see DetectChanges.cmd for an explanation of the funny stuff with ping etc. that's going on here
rem # returns the version of GIT installed
rem #
rem # ********* WARNING *********
rem # this script must be run in tests before any other git script so that the email and user name
rem # can be configured
rem # this is required by AppVeyor
rem # ***************************
echo dasmeta %time% %0
echo dasmeta_output_start
echo dasmeta_errors_start
rem # git config user.email "mikemay@blueyonder.co.uk" 2>&1
rem # if errorlevel 1 goto err_exit
rem # git config user.naem "Mike May" 2>&1
rem # if errorlevel 1 goto err_exit
git --version 2>&1
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%
:err_exit
exit %errorlevel%