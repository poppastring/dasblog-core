@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # see DetectChanges.cmd for an explanation of the funny stuff with ping etc. that's going on here
rem # returns the version of GIT installed
echo dasmeta %time% %0
echo dasmeta_output_start
echo dasmeta_errors_start
git config user.email "mikemay@blueyonder.co.uk" 2>&1
if errorlevel 1 goto err_exit
git config user.naem "Mike May" 2>&1
if errorlevel 1 goto err_exit
git --version 2>&1
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%
:err_exit
exit %errorlevel%