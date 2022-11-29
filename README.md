<p align="center">
  <a href="https://github.com/poppastring/dasblog-core">
    <img src="https://github.com/poppastring/dasblog-core/blob/main/images/dasblog.jpg" alt="DasBlog" />
  </a>
</p>
<p align="center">
	<a href="https://github.com/poppastring/dasblog-core/blob/main/FAQ.md">FAQ</a> |
	<a href="https://github.com/poppastring/dasblog-core/wiki/1.-Deployment">Deployment</a> |
	<a href="https://www.poppastring.com/blog/category/dasblog-core">Blog</a> |
	<a href="https://github.com/poppastring/dasblog-core/blob/main/CONTRIBUTING.md">Contributing to DasBlog</a>
	<br /><br />
	<a href="https://github.com/poppastring/dasblog-core/releases/">
		<img src="https://img.shields.io/github/v/release/poppastring/dasblog-core.svg" alt="Latest release" />
	</a>
	<a href="https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=main">
		<img src="https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20windows" alt="Windows Build status" />
	</a>
	<a href="https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=main">
		<img src="https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20linux" alt="Linux Build status" />
	</a>
</p>

# DasBlog
One of the primary goals of this project is to create a new blogging engine that preserves the essence of the original [DasBlog Blogging Engine](https://msdn.microsoft.com/en-us/library/aa480016.aspx), we also get the opportunity to take advantage of the modern cross platform goodness of ASP.NET Core.

## Building 
If you want to build and contribute code to DasBlog please fork this repo and submit a PR, check out the [contribution docs here](https://github.com/poppastring/dasblog-core/blob/main/CONTRIBUTING.md#developers) for more details.

## Deployment

You can deploy anywhere where .NET Core is hosted.

### Easiest Way: Click The Deploy To Azure Button

Click the button below, answer some questions, and be up and running on your DasBlog instance in a few minutes. Once you've deployed, be sure to read the [post-deployment instructions here](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment#2-configure-dasblog-core-settings).

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpoppastring%2Fdasblog-core%2Fmain%2Fdeploy%2Fazuredeploy.json)

### Deploying Manually
* [Deploying to Azure App Services for Windows](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment#deploy-to-azure-app-services-for-windows)
* [Deploying to Azure App Services for Linux](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment#deploy-to-azure-app-services-for-linux)
* [Deploying to a web host](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment#deploy-to-your-own-web-host)

After deploying the app you should immediately [update  the security settings](https://github.com/poppastring/dasblog-core/wiki/2.-Configure-your-blog).   

## Documentation
Check out the [wiki](https://github.com/poppastring/dasblog-core/wiki) for additional information on DasBlog fundamentals, architecture and themes.

Please [submit an issue](https://github.com/poppastring/dasblog-core/issues) if you encounter any problems.
