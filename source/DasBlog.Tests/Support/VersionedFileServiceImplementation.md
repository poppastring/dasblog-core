#### Versioned File Service Implementation
The versioned file service is implemented in [GitVersionedFileService](Interfaces/IVersionedFileService.cs).

It is based on Git.  It is implemented using cmd.exe rather than libgit2sharp as the author was
put off by the library's approach to Git.  That turned out to be a mistake for the development
of th feature and possibly as regards future use but swapping over should be relatively painless
if somebody develops a libgit2sharp version.

The interface with cmd.exe is handled through the `RunCmdProcess()` method of [ScriptRunner](Interfaces/IScriptRunner.cs).

##### See also
- [Versioned File Service User Guide](VersionedFileSystemUserGuide.md)