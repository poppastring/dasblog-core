#!/bin/bash
# Mike May - October 2018
# Script to run a git diff against a portion of the working tree and return output containing modification names
# if the working directory differs from the last commit
#
# could not use 'diff' - on linux (wsl) line endings are a problem that turned out to be intractable (short of converting the whole repo)
# git diff '--name-only' piped all the CRLF warnings into stdout 
#
# usage: cmd /c ./DetectChanges.sh <root of test resources>
# returns: non-empty output means that the working directory is different to the repo. 
# e.g. bash -c "/Users/mikedamay/projects/dasblog-core/source/DasBlog.Tests/Support/Scripts/bash/DetectChanges.sh /Users/mikedamay/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments"
# results will look something like the following if the working directory varies from the repo
# M ../../../Resources/Resources.csproj
# M ../../../Resources/Utils/LockFile.cs
# ?? ../../../Resources/aaa
#
# nothing is output if the working directory is clean
#
DT=`date`
echo dasmeta ${DT} $0 $@
echo dasmeta_output_start
echo dasmeta_errors_start
if [[ $# -eq 0 ]]
then
    echo "working directory path was blank)"
    echo dasmeta_errors_complete
    exit 1
fi
git status --short --untracked-files -- $1
echo dasmeta_output_complete errorlevel==$?
exit $?
