#### Versioned File Service User Guide

##### Usage
See [Anatomy of A Component Test](../FunctionalTests/ComponentTests/ComponentTests.md#anatomy-of-a-component-test)

##### Purpose
The role of the versioned file system is to provide a well defined starting point for each Component or Browser
Based Test.  This can be extended to other types of test.

A number of environments are provided such as "Vanilla" or "Language".  Each environment
has configuration and data content files set up appropriate to the tests that use them.  In reality
it's more a question of creating environments to avoid breaking existing tests.  For example if
you have built a test to return all the blog posts available then you don't want to screw up
that test next time you add a new post for some new test.

"Vanilla" was the first one created.  The intention with "Language" is to exercise those
bits of the code that are sensitive to the browser culture.

In order to limit the multiplicity of environments the functional tests' support classes include
an implementation of ITestDataProcessor which can manipulate the environment before a test begins.
This would typically be used to change config values but can also be used to add, update
or delete a blog post or other data.

##### Life Cycles and Work Flows
Each environment is is stored in the dasblog-core repo.

Note that most git commands will allow an optional path so a command such as
```
git status -- source/Dasblog.Tests/Resources/Environments
```
so, even though I am in the middle of editing this document, the status shows `Nothing to commit. Working tree clean`.

New environments are added by simply adding files and directories to the file system (or cloning an existing environment)
and then adding and committing them to the repo.

At the beginning of each test `ComponentTest.CreateSandBox()` is called and string passed in identifying which
of the environments is required.  `CreateSandBox()` checks that git is installed and that there are no
modifications to the working tree for the environments.  If the tree has been modified then an excepton is
thrown and the test aborted.  Other tests in the suite will run.

Objects created by the SUT such as the `BlogManager` are initialised with the appropriate environment.

On completion of the test the sand box's `Dispose()` method is called which stashes any modified files
and ensures the working tree is clean for the next test.

If you need to inspect the state of the files after the test then inspect the logs and get hold of the hash
for the stash in question.  Then it's a matter of `git stash apply <stash-hash>` which will put
the environment in its end of test state.

##### Pruning
Over time the number of stashes builds up.  

To inspect the state of play do `git fsck --unreachable`.  `git prune` will **_DELETE ALL STASHES_**.  If you want to be
more selective you can rely on these automatically created stashes having a description such as the following:
```
 On test/test-infrastructure-docs: functional-test environment state 932924a2-c930-4d7e-ad5a-fd5a9b149aa5
```
"functional-test environment state" followed by an unique GUID
will always be part of the description.  I'm not entirely sure what you can
do with this information but it may be useful if you are in trouble.

##### GIT Version
The earliest version of Git supported is 2.15.

##### See also
- [Versioned File Service Implementation](Interfaces/IVersionedFileService.cs)
