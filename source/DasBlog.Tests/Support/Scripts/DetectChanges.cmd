@echo off
rem #!/bin/bash
rem # Mike May - September 2018
rem # Script to run a git diff against a portion of the working tree and return a non-sero error code
rem # if the working directory differs from the last commit
rem #
rem # I wanted this to be a bash script for xplatform compatibility but wsl Ubuntu has only git v1.9 which
rem # does not include the all important 'git stash push' used elsewhere.  I hope that by the time
rem # we do the xplatform port wsl will be on something like git v2.15
rem #
rem # could not use 'diff' - on linux (wsl) line endings are a problem that turned out to be intractable (short of converting the whole repo)
rem # git diff '--name-only' piped all the CRLF warnings into stdout 
rem #
rem # usage: cmd /c ./DetectChanges.sh <script exit timeout> <root of test resources>
rem # returns: non-empty output means that the working directory is different to the repo. 
rem # e.g. "cmd /c ./DetectChanges.cmd 100 c:/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla"
if [%1] == [] goto error_exit
if [%2] == [] goto error_exit
echo %1
git status --short --untracked-files -- %2
rem # results will look something like the following if the working directory varies from the repo
rem # M ../../../Resources/Resources.csproj
rem # M ../../../Resources/Utils/LockFile.cs
rem # ?? ../../../Resources/aaa
rem # timeout /t 1 does not work where stdout/stderr is redirected
set exitcode=%errorlevel%
rem # this is required as a timeout so that cmd.exe lingers long enough for stdout and stderr output to be captured
rem # default is current 10 ms but it can be overriden with the DAS_BLOG_TEST_SCRIPT_EXIT_TIMEOUT=n env var
rem # where n is the number of milliseconds 
ping 192.168.0.255 -n 1 -w %1 >NUL
exit %exitcode%
:error_exit
echo working directory path was blank (or the script exit timeout was omitted)1>&2
rem # timeout /t 1
ping 192.168.0.255 -n 1 -w %1 >NUL
exit 1