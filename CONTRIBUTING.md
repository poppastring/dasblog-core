# Contributing to dasblog-core
DasBlog-core  is an open source project based heavily on the  work produced by a community of volunteers and contributors who helped make [the original DasBlog blogging engine](https://github.com/shanselman/dasblog) a roaring success. This project has adopted the [code of conduct policy](https://github.com/poppastring/dasblog-core/blob/main/CODE_OF_CONDUCT.md) to clarify expected behavior in our community.


## Issues
If you have found a bug or have an idea for a new feature/enhancement then please take a look at the Issues. You can search the Issues to see if someone has a similar problem or idea by using the Filter box. If your issue is a new one then please create it.

When creating issues, please provide as much information as possible.  The following information is recommended:
 - Any details of the error that you saw, screen shots of error dialogs are great. You can get access to your Open Live Writer logs by going to File, About Open Live Writer, Show log file.    If you can see an error message that looks to be related to your issue then please include details of that as well.

 - Detailed steps to be able to reproduce the error that you found.
 
 - The operating system and version you are running dasblog-core on (i.e. Linux, Windows Server 2019, etc. ).


## Developers
If you would like to discuss a change you are thinking of doing before you start work on it then feel free to raise an issue.

### Install the tools
To contribute code changes install the following dev tools:

- Install [Visual Studio Community 2022 (17.0 or newer)](https://visualstudio.microsoft.com/downloads/)
- Visual Studio installs the [.NET SDK 8.0.100](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))

### Contribute code
To contribute code to the project simply:
  1. Fork the repo and clone locally (ensure that you have [Git](https://git-scm.com/downloads) installed)
  2. Change to the "source" directory and open the *DasBlog All.sln* and perform a build.
  3. Create a specific topic branch, add a nice feature or fix your bug
  4. Send a Pull Request and we will start to discuss how your changes fit in.

### Architecture and Testing
If you are looking to take a more proactive role and want to help design decisions please let me know by submitting issues. You can help by get involved by reviewing Issues and PRs. Actively running Selenium tests locally is also a huge help!

#### Selenium install requirements
- [Java](https://java.com/en/download/windows_manual.jsp)
- [Node.js](https://nodejs.org/en/download/)
- [Google Chrome](https://www.google.com/chrome/)

Run the following from the command line:

`npm install -g selenium-standalone@latest`

`selenium-standalone install`

Open [Test Explorer in Visual Studio](https://docs.microsoft.com/visualstudio/test/run-unit-tests-with-test-explorer) and you can run any of the tests.

