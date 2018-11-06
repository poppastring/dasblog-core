@echo off
rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # see DetectChanges.cmd for an explanation of the funny stuff with bash, ping etc. that's going on here
rem #
rem # stashes the content of the path (typically the root of test data) and thereby resets the working directory
rem # to the last commit.  
rem # It then drops stash@{0} which will output the hash of the stash to stdoutput where it is grabbed and logged
rem #
rem # Would like to use 'git create' but it does not take a path spec.  So Heath Robinson measures are required...
rem #
rem # $1 = root of test resources
rem # $2 = string to identify the stash
rem # usage cmd TakeSnapshot.cmd <path-spec: root of test resources> <unique-id> <display-name>
rem # e.g. cmd /c StashCurrentState.cmd C:\alt\projs\dasblog-core\source\DasBlog.Tests\Resources Shearch_withBlankText_ShowsError 71923

rem # *********** WARNING ***************
rem # before this script is executed DetectChanges.cmd should be executed to ensure that there is something
rem # to stash.  Otherwise the DANGER is that some user invoked stash will be dropped by the second step,
rem # git stash drop command is executed.
rem # ***********************************
echo dasmeta %time% %0 %1 %2
echo dasmeta_output_start
echo dasmeta_errors_start
if [%1] == [] goto err_exit
if [%2] == [] goto err_exit
git stash push -m "functional-test environment state %2" --all -- %1 2>&1
if errorlevel 1 goto error_exit2
rem # "drop" will cause the hash of the stash to be echoed to stdout where the caller can grab it and tell user
git stash drop stash@{0} 2>&1
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%

:error_exit
echo one or more command line arguments are missing

:error_exit2
echo dasmeta_errors_complete
exit 1