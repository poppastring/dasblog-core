#!/usr/bin/env bash
# Mike May - October 2018
#
# prints out the message associated with the stash so that the caller can verify it.
# $1 = the hash of a stash
# usage bash -c "./ConfirmStash.sh <git-stash-hash>"
# e.g. bash -c "/Users/mikedamay/projects/dasblog-core/source/DasBlog.Tests/Support/Scripts/bash/ConfirmStash.sh 494058f3f10c9b47f354f02f175c7498393afe91"
# output "On test/test-infrastructure-xplatform: functional-test environment state xxx-yyyy" followed by a newline
DT=`date`
echo dasmeta ${DT} $0 $@
echo dasmeta_output_start
echo dasmeta_errors_start
if [[ $# -eq 0 ]] 
then 
    echo one or more command line arguments are missing
    echo dasmeta_errors_complete
    exit 1
fi
git log --format=%B -n 1 $1
echo dasmeta_output_complete errorlevel==$?
exit $?
