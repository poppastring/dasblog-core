#### Cross-platform

##### Goals
The intention is to port the app to MacOs and Linux in [Phase 3 of the project](https://github.com/poppastring/dasblog-core/projects)

One of the phase 2 goals is to increase test coverage and it is felt that a certain amount of cross-platform work
should be done at this stage to ensure that tests are designed in a manner appropriate for cross-platform
development ensuring that the final port will be relatively smooth.

##### Status - November 2018
A "Posix" configuration is available.

The posix configuration includes all web-site functionality currently available on the windows version.  It excludes
back-end operations such as RSS and Email.

###### Windows
The posix configuration is fully functional on Windows (Windows 10 Pro 1803 17134.345) in line with the scope summarised in the previous paragraph and
all tests run and pass.  The major difference between the posix and windows configuration is that the latter includes the
legacy newteillgence code and associated support assemblies.  All cross-platform decisions other than the exclusion
of legacy code are handled at run-time.  All tests pass on Windows.  The SmokeTest is also fully functional.

###### MacOs
The posix configuration is fully functional on MacOs (10.13.6) subject to the above mentioned scope.  All tests
pass except one Test Infrastructure test fail and all 4 Browser Based tests.  No attempt has been made yet
to get Selenium operational on a posix platform.

###### Linux
The posix configuration fails to build on Linux (Centos 7.5.1804 (Core)).

The killer error message is
```
usr/share/dotnet/sdk/NuGetFallbackFolder/microsoft.aspnetcore.razor.design/2.1.2/build/netstandard2.0/Microsoft.AspNetCore.Razor.Design.CodeGeneration.targets(121,5): error : rzc generate exited with code 1. 
[/home/mike/projects/dasblog-core/source/DasBlog.Web.UI/DasBlog.Web.csproj]
```
Presumably this is a dotnet SDK or Runtime mismatch.  If not, I have seen dark mutterings on the
web about Razor Generation issues being a mismatch between SDK version and nuget package versions so
here's hoping.

##### Installation and Build

###### Windows
Build with the "posix" configuration and run tests with "--no-build" or some variation.  No platform specific
flag or environment variable is required to run the web app in normal interactive mode.

dotnet build -c posix

Note that no configuration has been attempted under WSL (Windows Subsystem for Linux).  If you do attempt this
you will have upgrade git to 2.15+.  On 1803 this is currently v1.9.

###### MacOs
Tested on v10.13.6

install git (v2.15+) and dotnet (v2.1.403 currently). 

Git can be installed using Homebrew or some variation.  dotnet can be [downloaded](https://www.microsoft.com/net/download/dotnet-core/2.1).

```
cd <project dir>/source
dotnet cleAN
dotnet test -c posix
```

###### Linux
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

To build
```
cd <project dir>/source
dotnet build -c posix
```
results in the razor generation error detailed above.


##### Process
The tests of the posix version should be run on a non-Windows platform once a week and be kept up-to-date.