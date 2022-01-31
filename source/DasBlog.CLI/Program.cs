using System;
using System.IO;
using System.Linq;
using ConsoleTables;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using DasBlog.Services.Users;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DasBlog.CLI
{
    class Program
    {
		public const string ASPNETCORE_ENV_NAME = "ASPNETCORE_ENVIRONMENT";
		public static string ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable(ASPNETCORE_ENV_NAME);
		public static string CONFIG_DIRECTORY = Path.Combine(Environment.CurrentDirectory, "Config");
		public const string ADMINPASSWORD = "19-A2-85-41-44-B6-3A-8F-76-17-A6-F2-25-01-9B-12";
		public static string SITECONFIG_FILENAME = string.Empty;
		public static string SITESECURITYCONFIG_FILENAME = string.Empty;

		public static IConfiguration Configuration { get; set; }

		static int Main(string[] args)
        {
			if (string.IsNullOrWhiteSpace(ASPNETCORE_ENVIRONMENT))
			{
				ASPNETCORE_ENVIRONMENT = "Production";
			}

			DefineConfigNames();

			Configuration = DasBlogConfigurationBuilder();

			var service = new ServiceCollection();

			service.Configure<ConfigFilePathsDataOption>(options =>
			{
				options.SiteConfigFilePath = Path.Combine(CONFIG_DIRECTORY, SITECONFIG_FILENAME);
				options.SecurityConfigFilePath = Path.Combine(CONFIG_DIRECTORY, SITESECURITYCONFIG_FILENAME);
				options.ThemesFolder = Path.Combine(Environment.CurrentDirectory, "Themes");
			});

			service
				.Configure<SiteConfig>(Configuration)
				.AddSingleton<IUserDataRepo, UserDataRepo>()
				.AddSingleton<IUserService, UserService>()
				.AddSingleton<IConfigFileService<SiteConfig>, SiteConfigFileService>()
				.AddSingleton<IConfigFileService<SiteSecurityConfigData>, SiteSecurityConfigFileService>()
				.BuildServiceProvider();

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

						Console.WriteLine($"Environment variable has been set to '{val.Value}'.");
					});
				});

				configCmd.Command("root", setCmd =>
				{
					setCmd.Description = "Required: Set the base URL value for the site e.g. http://www.somesite.com";
					var val = setCmd.Argument("value", "Value of 'root' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						var serviceProvider = service.BuildServiceProvider();
						var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;
						sc.Root = val.Value;

						var fs = serviceProvider.GetService<IConfigFileService<SiteConfig>>();
						if (fs.SaveConfig(sc))
						{
							Console.WriteLine($"Site 'root' has been set to '{val.Value}'.");
						}
						else
						{
							Console.WriteLine($"Save failed!");
						}
					});
				});

				configCmd.Command("theme", setCmd =>
				{
					setCmd.Description = "Change the site theme";
					var val = setCmd.Argument("value", "Value of 'theme' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						var serviceProvider = service.BuildServiceProvider();
						var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;
						sc.Theme = val.Value;

						var fs = serviceProvider.GetService<IConfigFileService<SiteConfig>>();
						if (fs.SaveConfig(sc))
						{
							Console.WriteLine($"Site 'theme' has been set to '{val.Value}'.");
						}
						else
						{
							Console.WriteLine($"Save failed!");
						}
					});
				});

				configCmd.Command("contentdir", setCmd =>
				{
					setCmd.Description = "Change the site content directory location";
					var val = setCmd.Argument("value", "Value of 'contentdir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						var serviceProvider = service.BuildServiceProvider();
						var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;
						sc.ContentDir = val.Value;

						var fs = serviceProvider.GetService<IConfigFileService<SiteConfig>>();
						if (fs.SaveConfig(sc))
						{
							Console.WriteLine($"Site 'contentdir' has been set to '{val.Value}'.");
						}
						else
						{
							Console.WriteLine($"Save failed!");
						}	
					});
				});

				configCmd.Command("binarydir", setCmd =>
				{
					setCmd.Description = "Change the site binary directory location";
					var val = setCmd.Argument("value", "Value of 'binarydir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						var serviceProvider = service.BuildServiceProvider();
						var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;
						sc.BinariesDir = val.Value;

						var fs = serviceProvider.GetService<IConfigFileService<SiteConfig>>();
						if (fs.SaveConfig(sc))
						{
							Console.WriteLine($"Site 'binarydir' has been set to '{val.Value}'.");
						}
						else
						{
							Console.WriteLine($"Save failed!");
						}
					});
				});

				configCmd.Command("logdir", setCmd =>
				{
					setCmd.Description = "Change the site log directory location";
					var val = setCmd.Argument("value", "Value of 'logdir' parameter").IsRequired();
					setCmd.OnExecute(() =>
					{
						var serviceProvider = service.BuildServiceProvider();
						var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;
						sc.LogDir = val.Value;

						var fs = serviceProvider.GetService<IConfigFileService<SiteConfig>>();
						if (fs.SaveConfig(sc))
						{
							Console.WriteLine($"Site 'logdir' has been set to '{val.Value}'.");
						}
						else
						{
							Console.WriteLine($"Save failed!");
						}

					});
				});
			});

			app.Command("init", initCmd =>
			{
				initCmd.Description = "Initializing the site creates environment specific config files (Staging or Production)";
				initCmd.OnExecute(() =>
				{
					if (!InitializeConfigFiles.IsInitialized(CONFIG_DIRECTORY, ASPNETCORE_ENVIRONMENT))
					{
						InitializeConfigFiles.CopyFiles(CONFIG_DIRECTORY, ASPNETCORE_ENVIRONMENT);
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
					var serviceProvider = service.BuildServiceProvider();
					var sc = serviceProvider.GetService<IOptions<SiteConfig>>().Value;

					var configfs = serviceProvider.GetService<IOptions<ConfigFilePathsDataOption>>().Value;

					var table = new ConsoleTable("Settings", "Value");
					table.AddRow("Site Initialized?", InitializeConfigFiles.IsInitialized(CONFIG_DIRECTORY, ASPNETCORE_ENVIRONMENT))
								.AddRow("Environment", ASPNETCORE_ENVIRONMENT)
								.AddRow("Site file", SITECONFIG_FILENAME)
								.AddRow("SiteSecurity file", SITESECURITYCONFIG_FILENAME)
								.AddRow("root", sc.Root)
								.AddRow("theme", sc.Theme)
								.AddRow("contentdir", sc.ContentDir)
								.AddRow("binarydir", sc.BinariesDir)
								.AddRow("logdir", sc.LogDir);

					table.Write();
				});

			});

			app.Command("resetpassword", resetCmd =>
			{
				resetCmd.Description = "**WARNING** Resets all user passowrds to 'admin'";
				resetCmd.OnExecute(() =>
				{
					var serviceProvider = service.BuildServiceProvider();
					var userService = serviceProvider.GetService<IUserService>();

					var users = userService.GetAllUsers().ToList();

					users.ForEach(x => x.Password = ADMINPASSWORD);

					var fs = serviceProvider.GetService<IConfigFileService<SiteSecurityConfigData>>();
					if (fs.SaveConfig(new SiteSecurityConfigData() { Users = users }))
					{
						Console.WriteLine("All passwords reset to 'admin'");
					}
					else
					{
						Console.WriteLine($"Reset failed!");
					}
					
				});

			});

			app.Command("newtheme", createthemeCmd =>
			{
				createthemeCmd.Description = "Creates a new theme based on the default 'dasblog' theme";
				var val = createthemeCmd.Argument("value", "Name of the new theme").IsRequired();
				createthemeCmd.OnExecute(() =>
				{
					// Execute command
					var serviceProvider = service.BuildServiceProvider();
					var fs = serviceProvider.GetService< IOptions<ConfigFilePathsDataOption>>();

					var fileOptions = fs.Value;

					if (DirectoryCopy(Path.Combine(fileOptions.ThemesFolder, "dasblog"), Path.Combine(fileOptions.ThemesFolder, val.Value), true))
					{
						Console.WriteLine($"New theme created: '{val.Value}' ");
					}
					
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

		private static void DefineConfigNames()
		{
			if(!string.IsNullOrWhiteSpace(ASPNETCORE_ENVIRONMENT))
			{
				SITECONFIG_FILENAME = string.Format("site.{0}.config", ASPNETCORE_ENVIRONMENT);
				SITESECURITYCONFIG_FILENAME = string.Format("siteSecurity.{0}.config", ASPNETCORE_ENVIRONMENT);

				if (!new FileInfo(Path.Combine(CONFIG_DIRECTORY, SITECONFIG_FILENAME)).Exists)
				{
					SITECONFIG_FILENAME = "site.config";
				}

				if (!new FileInfo(Path.Combine(CONFIG_DIRECTORY, SITESECURITYCONFIG_FILENAME)).Exists)
				{
					SITESECURITYCONFIG_FILENAME = "siteSecurity.config";
				}
			}
			else
			{
				SITECONFIG_FILENAME = "site.config";
				SITESECURITYCONFIG_FILENAME = "siteSecurity.config";
			}
		}

		private static IConfiguration DasBlogConfigurationBuilder()
		{
			var configBuilder = new ConfigurationBuilder();

			configBuilder
				.AddXmlFile(Path.Combine(CONFIG_DIRECTORY, SITECONFIG_FILENAME), optional: false, reloadOnChange: true);

			return configBuilder.Build();
		}

		private static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			var dir = new DirectoryInfo(sourceDirName);
			if (!dir.Exists)
			{
				Console.WriteLine($"Source theme ('dasblog') does not exist or could not be found:");
				return false;
			}

			var dir2 = new DirectoryInfo(destDirName);
			if (dir2.Exists)
			{
				Console.WriteLine($"New theme name already exists");
				return false;
			}

			var dirs = dir.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			var files = dir.GetFiles();
			foreach (var file in files)
			{
				var temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			if (copySubDirs)
			{
				foreach (var subdir in dirs)
				{
					var temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}

			return true;
		}


	}
}
