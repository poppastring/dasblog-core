using System;
using System.IO;
using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;

namespace DasBlog.CLI
{
    class Program
    {
		public const string ASPNETCORE_ENV_NAME = "ASPNETCORE_ENVIRONMENT";
		public static string ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENV_NAME");
		public const string ADMINPASSWORD = "19-A2-85-41-44-B6-3A-8F-76-17-A6-F2-25-01-9B-12";

		static int Main(string[] args)
        {

			var app = new CommandLineApplication
			{
				Name = "dasblog-core",
				Description = "Configure DasBlog Core from the CLI.",
			};

			app.HelpOption(inherited: true);
			app.Command("config", configCmd =>
			{
				configCmd.Description = "Allows updates to the primary configuration settings";

				configCmd.OnExecute(() =>
				{
					Console.WriteLine("Specify a subcommand");
					configCmd.ShowHelp();
					return 1;
				});

				configCmd.Command("env", setCmd =>
				{
					setCmd.Description = "Required: Set the environment variable e.g. Development, Staging or Production";
					var val = setCmd.Argument("value", "Value of 'environment' parameter");
					setCmd.OnExecute(() =>
					{
						if (!string.IsNullOrWhiteSpace(val.Value))
						{
							Environment.SetEnvironmentVariable(ASPNETCORE_ENV_NAME, val.Value);
						}
						else
						{
							Environment.SetEnvironmentVariable(ASPNETCORE_ENV_NAME, null);
						}

						Console.WriteLine($"Environment variable has been set to '{val.Value}', restart site for operation to take effect");
					});
				});

				configCmd.Command("root", setCmd =>
				{
					setCmd.Description = "Required: Set the base URL value for the site e.g. http://www.somesite.com";
					var val = setCmd.Argument("value", "Value of 'root' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						// Execute command
						Console.WriteLine($"Site 'root' has been set to '{val.Value}', restart site for operation to take effect");
					});
				});

				configCmd.Command("theme", setCmd =>
				{
					setCmd.Description = "Change the site theme";
					var val = setCmd.Argument("value", "Value of 'theme' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						// Execute command
						Console.WriteLine($"Site 'theme' has been set to '{val.Value}', restart site for operation to take effect");
					});
				});

				configCmd.Command("contentdir", setCmd =>
				{
					setCmd.Description = "Change the site content directory location";
					var val = setCmd.Argument("value", "Value of 'contentdir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						// Execute command
						Console.WriteLine($"Site 'contentdir' has been set to '{val.Value}', restart site for operation to take effect");
					});
				});

				configCmd.Command("binarydir", setCmd =>
				{
					setCmd.Description = "Change the site binary directory location";
					var val = setCmd.Argument("value", "Value of 'binarydir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						// Execute command
						Console.WriteLine($"Site 'binarydir' has been set to '{val.Value}', restart site for operation to take effect");
					});
				});

				configCmd.Command("logdir", setCmd =>
				{
					setCmd.Description = "Change the site logd directory location";
					var val = setCmd.Argument("value", "Value of 'logdir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						// Execute command
						Console.WriteLine($"Site 'logdir' has been set to '{val.Value}', restart site for operation to take effect");
					});
				});
			});

			app.Command("initialize", initCmd =>
			{
				initCmd.Description = "Initializing the site creates environment specific config files (Staging or Production)";
				initCmd.OnExecute(() =>
				{
					if (!InitializeConfigFiles.IsInitialized(Path.Combine(Environment.CurrentDirectory, "Config"), ASPNETCORE_ENVIRONMENT))
					{
						InitializeConfigFiles.CopyFiles(Path.Combine(Environment.CurrentDirectory, "Config"), ASPNETCORE_ENVIRONMENT);
						Console.WriteLine($"Site settings files have been been initialized for this site! (Environment = {ASPNETCORE_ENVIRONMENT})");
					}
					else
					{
						Console.WriteLine("Site settings files have already been been initialized for this site");
					}
				});
			});

			app.Command("environment", envCmd =>
			{
				envCmd.Description = "List the main environment settings associated with DasBlog Core";
				envCmd.OnExecute(() =>
				{
					var table = new ConsoleTable("Settings", "Value");
					table.AddRow("Site Initialized?", InitializeConfigFiles.IsInitialized(Path.Combine(Environment.CurrentDirectory, "Config"), ASPNETCORE_ENVIRONMENT))
								.AddRow("Environment", ASPNETCORE_ENVIRONMENT)
								.AddRow("root", "")
								.AddRow("theme", "")
								.AddRow("contentdir", "")
								.AddRow("binarydir", "");

					table.Write();
				});

			});

			app.Command("resetpassword", resetCmd =>
			{
				resetCmd.Description = "Resets the admin password to 'admin'";
				resetCmd.OnExecute(() =>
				{

					// Reset password...

					Console.WriteLine("Reset password");
				});

			});

			app.Command("createtheme", createthemeCmd =>
			{
				createthemeCmd.Description = "Creates a new theme based on the default dasblog theme";
				var val = createthemeCmd.Argument("value", "Name of the new theme").IsRequired();
				createthemeCmd.OnExecute(() =>
				{
					// Execute command


					Console.WriteLine($"New theme created: '{val.Value}' ");
				});
			});


			app.OnExecute(() =>
			{
				Console.WriteLine("Specify a subcommand");
				app.ShowHelp();
				return 1;
			});

			return app.Execute(args);
		}
	}
}
