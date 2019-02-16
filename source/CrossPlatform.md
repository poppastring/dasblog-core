#### Cross-platform

##### Goals
The intention is for DasBlog-Core to run under Windows, Linux and MacOS.  This is a phase 3 activity.  The work described
below (in phase 2) relates to ensuring there is an amenable codebase for the move to cross platform support.

##### Status - February 2019
The application front-end currently builds and runs under Windows (10 1803+) MacOS (10.14.3+) and Linux (CentOs 7.5.1804 / Kernel 3.10).
The status of non-front-end features such as RSS is not currently known.  No platform specific flags are used anywhere.

Browser based tests currently fail on MacOS and Linux

##### Installation and Build
Guidance below discusses running with the embedded Kestrel server endpoint directly exposed to clients.  This document
does not discuss configurations involving IIS or IIS XPress.

###### Build
Change directory to &lt;project dir&gt;/source (where project directory is typically dasblog-core).

do `dotnet build`

###### Test
Change directory to &lt;project dir&gt; (where project directory is typically dasblog-core).

To run unit tests do `dotnet test source/DasBlog.Tests/UnitTests --logger trx;LogfileName=test_results.xml --results-directory ./test_results --filter Category=UnitTest`

To run component tests do `dotnet test source/DasBlog.Tests/FunctionalTests --logger trx;LogfileName=component_test_results.xml --results-directory ./test_results --filter Category=ComponentTest`

Browser based tests are not currently functional on Linux or MacOS.  For Windows refer to the test documentation.

###### Run
Change directory to &lt;project dir&gt;/source/DasBlog.Web.UI (where project directory is typically dasblog-core).

from bash do:
```
export DAS_BLOG_OVERRIDE_ROOT_URL=1
dotnet run
```
or from Windows/cmd do:
```
set DAS_BLOG_OVERRIDE_ROOT_URL=1
dotnet run
```

In the browser go to localhost:50432/ - you should see the familiar home page.

Note that no configuration has been attempted under WSL (Windows Subsystem for Linux).  If you do attempt this
you will have upgrade git to 2.15+.  On 1803 this is currently v1.9.


###### Installation - MacOS
Tested on v10.14.3

install git (v2.15+) and dotnet (v2.1.403 currently). 

Git can be installed using Homebrew or some variation.  dotnet can be [downloaded](https://www.microsoft.com/net/download/dotnet-core/2.1).

###### Installation - Linux
Tested on Centos 7.5.1804

To install git:
```
sudo yum install http://opensource.wandisco.com/centos/7/git/x86_64/wandisco-git-release-7-2.noarch.rpm
sudo yum install git
```

To install dotnet
```
TBA
```
