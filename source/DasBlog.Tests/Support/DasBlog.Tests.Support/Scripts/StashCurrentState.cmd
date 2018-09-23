rem #!/usr/bin/env bash
rem # Mike May - September 2018
rem # stashes the content of the path (typically the root of test data) and thereby resets the working directory
rem # to the repo.  Then tags the stash with display name + unique id so that it can be applied manually if necessary from the tag
rem # with git stash apply display-name_unique-id.
rem #
rem # Would like to use 'git create' but it does not take a path spec.  So Heath Robinson measures are required...
rem #
rem # $1 = root of test resources
rem # $2 = string to identify the stash
rem # $3 = tag name
rem # usage cmd TakeSnapshot.cmd <path-spec: root of test resources> <unique-id> <display-name>
rem # e.g. cmd /c StashCurrentState.cmd C:\alt\projs\dasblog-core\source\DasBlog.Tests\Resources Shearch_withBlankText_ShowsError 71923
echo %0
if [%1] == [] goto err_exit
if [%2] == [] goto err_exit
if [%3] == [] goto err_exit
git stash push -m "%2 functional-test environment state" --all -- %1
if eroorlevel 1 exit %errorlevel%

git tag %3_$2 stash@{0}

exit %errorlevel%
:err_exit
echo one or more command line arguments are missing 2>&1
exit 1