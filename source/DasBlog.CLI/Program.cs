using System;

namespace DasBlog.CLI
{
    class Program
    {
		public static string ASPNETCORE_ENVIRONMENT = string.Empty;
		public const string ADMINPASSWORD = "19-A2-85-41-44-B6-3A-8F-76-17-A6-F2-25-01-9B-12";

		static void Main(string[] args)
        {
			if (args.Length == 0)
			{
				return;
			}

			ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			
			var command = args[0];

			// "help" (list all commends)
			// "help initialize" (explanation of initialize command)
			// "initialize" (creates config files base on environment)
			// "environment" (Lists the environment variable; configured directories; config file locations; root, theme, admin, list of themes);
			// "root https://www.poppastring.com/blog" (set root config setting)
			// "theme fulcrum" (set theme setting)
			// "contentdir content"
			// "binarydir content/binary"
			// "logdir logs"
			// "resetpassword" (reset admin password)
			// "newtheme business" (creates a new theme folder copied from dasblog)


			Console.WriteLine("Hello World!");
        }
    }
}
