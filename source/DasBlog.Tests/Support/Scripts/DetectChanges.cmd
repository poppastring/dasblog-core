@echo off
rem #!/bin/bash
rem # Mike May - September 2018
rem # Script to run a git diff against a portion of the working tree and return output containing modification names
rem # if the working directory differs from the last commit
rem #
rem # I wanted this to be a bash script for xplatform compatibility but wsl Ubuntu has only git v1.9 which
rem # does not include the all important 'git stash push' used elsewhere.  I hope that by the time
rem # we do the xplatform port wsl will be on something like git v2.15
rem #
rem # could not use 'diff' - on linux (wsl) line endings are a problem that turned out to be intractable (short of converting the whole repo)
rem # git diff '--name-only' piped all the CRLF warnings into stdout 
rem #
rem # usage: cmd /c ./DetectChanges.sh <root of test resources>
rem # returns: non-empty output means that the working directory is different to the repo. 
rem # e.g. "cmd /c ./DetectChanges.cmd c:/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla"
echo dasmeta %time% %0 %1
echo dasmeta_output_start
echo dasmeta_errors_start
if [%1] == [] goto error_exit
git status --short --untracked-files -- %1 2>&1
rem # results will look something like the following if the working directory varies from the repo
rem # M ../../../Resources/Resources.csproj
rem # M ../../../Resources/Utils/LockFile.cs
rem # ?? ../../../Resources/aaa
echo dasmeta_output_complete errorlevel==%errorlevel%
exit %errorlevel%
:error_exit
echo working directory path was blank (or the script exit timeout was omitted)
echo dasmeta_errors_complete
exit 1