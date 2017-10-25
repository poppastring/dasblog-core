using RJH.CommandLineHelper;

using System;


namespace RJH.CommandLineHelper.Test
{
	/// <summary>
	/// Summary description for CommandLine.
	/// </summary>
	public class CommandLine
	{
		[CommandLineSwitch("SourceType","Specify the blog program from which to import content.")]
		public BlogSourceType Source
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get 
			{ 
				return _source; 
			}  
			[System.Diagnostics.DebuggerStepThrough()]
			set
			{ 
				_source = value; 
			}  
		} 
		private BlogSourceType _source;

		[CommandLineSwitch("ContentDir", "The DasBlog content directory into which the entries are placed.")]
		[CommandLineAlias("to")]
		public string ContentDirectory
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get 
			{ 
				return _contentDirectory; 
			}  
			[System.Diagnostics.DebuggerStepThrough()]
			set
			{ 
				_contentDirectory = value; 
			}  
		} 
		private string _contentDirectory;

		[CommandLineSwitch("SourceDir", "The source directory from which content will be imported.")]
		[CommandLineAlias("from")]
		public string SourceDirectory
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get 
			{ 
				return _sourceDirectory; 
			}  
			[System.Diagnostics.DebuggerStepThrough()]
			set
			{ 
				_sourceDirectory = value; 
			}  
		} 
		private string _sourceDirectory;

		[CommandLineSwitch("Help", "Displays the command line help.")]
		public bool Help
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get 
			{ 
				return _help; 
			}  
			[System.Diagnostics.DebuggerStepThrough()]
			set
			{ 
				_help = value; 
			}  
		} 
		private bool _help = false;

	}
}
