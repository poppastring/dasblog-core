using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using DasBlog.Tests.Support;
using Constants = DasBlog.Tests.Support.Common.Constants;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public partial class TestDataProcesor : ITestDataProcessor
	{
		private string testDataPath;
		/// <param name="testDataPath">
		///   e.g. "c:/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla"
		/// </param>
		public TestDataProcesor(string testDataPath)
		{
			this.testDataPath = testDataPath;
		}
		/// <inheritdoc cref="ITestDataProcessor.GetValue"/>
		/// <param name="xPath">e.g. "/post:DayEntry/post:Entries/post:Entry[post:Title='abc']/post:EntryId'"
		/// ****** CAUTION ******
		/// Callers must prefix all the element names with 'post:' because the C# XPath mechanism
		/// insist that even the default namespace is specified in the xpath</param>
		public (bool success, string value) GetValue(string filePathRelativeToEnvironment, string xPath)
		{
			var xmlText = File.ReadAllText(CombinePaths(filePathRelativeToEnvironment));
			XPathDocument xdoc = new XPathDocument(new StringReader(xmlText));
			XPathNavigator xnav = xdoc.CreateNavigator();			
			var resolver = new XmlNamespaceManager(xnav.NameTable);
			resolver.AddNamespace("post","urn:newtelligence-com:dasblog:runtime:data");
			var iter = xnav.Select(xPath, resolver);
			if (!iter.MoveNext())
			{
				return (false, $"no values found in file for {xPath}");
			}
			var result = iter.Current.InnerXml;
			if (iter.MoveNext())
			{
				throw new Exception($"duplicate values encountered in file for {xPath}");
			}
			return (true, result);
		}

		/// <inheritdoc cref="ITestDataProcessor.GetSiteConfigValue"/>
		public (bool success, string value) GetSiteConfigValue(string key)
		{
			// e.g c:/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Config/site.config
			// e.g. "/SiteConfig/EnableTitlePermaLinkUnique"
			return GetValue(Constants.SiteConfigPathFragment, $"{Constants.SiteConfigRoot}/{key}");
		}
		/// <inheritdoc cref="ITestDataProcessor.GetSiteSecurityConfigValue"/>
		public (bool success, string value) GetSiteSecurityConfigValue(string email, string key)
		{
			// e.g c:/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Config/siteSecurity.config
			// e.g. "/SiteSecurityConfig/Users/User[EmailAddress='myemail@myemail.com']/Role"
			return GetValue(Constants.SiteSecurityConfigPathFragment
			  , $"{Constants.SiteSecurityConfigRoot}/Users/User[EmailAddress='{email}']/{key}");
		}
/*
		/// <inheritdoc cref="ITestDataProcessor.GetBlogPostValue"/>
		public (bool success, string value) GetBlogPostValue(DateTime dt, Expression<Func<Entry, bool>> pred, string key)
		{
			throw new NotImplementedException();
		}
*/
/*
		/// <inheritdoc cref="ITestDataProcessor.SetValue"/>
		public void SetValue(string filePathRelativeToEnvironment, params object[] pathPartsAndValue)
		{
			var xmlText = File.ReadAllText(CombinePaths(filePathRelativeToEnvironment));
			var xslText = string.Format(siteConfigTrasnsform.Trim(), pathPartsAndValue);
			XPathDocument xml = new XPathDocument(new StringReader(xmlText));
			XPathDocument xsl = new XPathDocument(new StringReader(xslText));
			var xslt = new XslCompiledTransform();
			xslt.Load(xsl);
			var writer = new StringWriter();
			xslt.Transform(xml, null, writer);
			string str = writer.ToString();
			File.WriteAllText(CombinePaths(filePathRelativeToEnvironment), str);
		}
*/
		/// <inheritdoc cref="ITestDataProcessor.SetSiteConfigValue"/>
		public void SetSiteConfigValue(string key, string value)
		{
			SetValueUsingTemplate(Constants.SiteConfigPathFragment, siteConfigTrasnsform, key, value);
		}
		/// <inheritdoc cref="ITestDataProcessor.SetSiteSecurityConfigValue"/>
		public void SetSiteSecurityConfigValue(string email, string key, string value)
		{
			SetValueUsingTemplate(Constants.SiteSecurityConfigPathFragment, siteSecurityConfigTransform, email, key, value);
		}
		/// <inheritdoc cref="ITestDataProcessor.SetBlogPostValue"/>
		public void SetBlogPostValue(DateTime dt, Expression<Func<Entry, bool>> pred, string key)
		{
			throw new NotImplementedException();
		}
		private string CombinePaths(string filePathRelativeToEnvironment)
		{
			return Path.Combine(testDataPath, filePathRelativeToEnvironment);
		}
		public void SetValueUsingTemplate(string filePathRelativeToEnvironment, string template, params object[] pathPartsAndValue)
		{
			var xmlText = File.ReadAllText(CombinePaths(filePathRelativeToEnvironment));
			var xslText = string.Format(template.Trim(), pathPartsAndValue);
			var str = RunTransform(xmlText, xslText);
			File.WriteAllText(CombinePaths(filePathRelativeToEnvironment), str);
		}

		private static string RunTransform(string xmlText, string xslText)
		{
			XPathDocument xml = new XPathDocument(new StringReader(xmlText));
			XPathDocument xsl = new XPathDocument(new StringReader(xslText));
			var xslt = new XslCompiledTransform();
			xslt.Load(xsl);
			var writer = new StringWriter();
			xslt.Transform(xml, null, writer);
			string str = writer.ToString();
			return "<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine + str;
		}
	}

	public interface ITestDataProcesorFactory
	{
		ITestDataProcessor CreateTestDataProcessor(IDasBlogSandbox sandbox);
	}
	public class TestDataProcesorFactory : ITestDataProcesorFactory
	{
		public ITestDataProcessor CreateTestDataProcessor(IDasBlogSandbox sandbox)
		{
			return new TestDataProcesor(sandbox.TestEnvironmentPath);
		}
	}
	
}
