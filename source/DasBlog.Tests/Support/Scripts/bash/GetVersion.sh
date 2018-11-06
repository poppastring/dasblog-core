#!/usr/bin/env bash
# Mike May - October 2018
# returns the version of GIT installed
#
# usage bash -c "GetVersion.sh"
# e.g. bash -c "/Users/mikedamay/projects/dasblog-core/source/DasBlog.Tests/Support/Scripts/bash/GetVersion.sh"
# outputs "git version 2.19.1"
echo dasmeta ${DT} $0 $@
echo dasmeta_output_start
echo dasmeta_errors_start
git --version
echo dasmeta_output_complete errorlevel $?
exit $?
