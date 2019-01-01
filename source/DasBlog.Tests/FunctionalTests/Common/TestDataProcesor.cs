using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using DasBlog.Tests.Support;
using Constants = DasBlog.Tests.Support.Common.Constants;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
		/// Callers must prefix all the element names for blog entries and comments with 'post:' because the C# XPath mechanism
		/// insist that even the default namespace is specified in the xpath</param>
		public (bool success, string value) GetValue(string filePathRelativeToEnvironment, string xPath)
		{
			try
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
			} finally {}
			// in 2 minds about throwing an exception. for no file found.  Maybe the callers in this claass
			// should catch the exception and return false.  I think there is a strong presumption that
			// if you specify the file as we do here that you would expect the file to exist
/*
			catch (Exception e)
			{
				return (false, e.Message);
			}
*/
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
		/// <inheritdoc cref="ITestDataProcessor.GetBlogPostValue"/>
		public (bool success, string value) GetBlogPostValue(DateTime dt, string entryId, string key)
		{
			string fileName = GetBlogEntryFileName(dt);
			return GetValue(Path.Combine(Constants.ContentDirectory, fileName)
				, $"/post:DayEntry/post:Entries/post:Entry[post:EntryId='{entryId}']/post:{key}");
		}
		/// <inheritdoc cref="ITestDataProcessor.GetBlogPostValue"/>
		public (bool success, XElement data) GetBlogPostFileContents(DateTime dt)
		{
			string fileName = GetBlogEntryFileName(dt);
			var path = CombinePaths(Path.Combine(Constants.ContentDirectory, fileName));
			if (!File.Exists(path))
				return (false, new XElement("empty"));
			var str = File.ReadAllText(path);
			return (true, XElement.Parse(str));
		}

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
		public void SetBlogPostValue(DateTime dt, Expression<Func<Entry, bool>> pred, string key, object value)
		{
			throw new NotImplementedException();
		}
		/// <inheritdoc cref="ITestDataProcessor.SetDayExtraValue"/>
		public (bool success, string value) GetDayExtraValue(DateTime dt, string entryId, string key)
		{
			return GetValue(Path.Combine(Constants.ContentDirectory, GetDayExtraFileName(dt))
			  , $"/post:DayExtra/post:Comments/post:Comment[post:EntryId=\"{entryId}\"]/post:{key}");
		}
		/// <inheritdoc cref="ITestDataProcessor.GetDayExtraFileContents"/>
		public (bool success, XElement data) GetDayExtraFileContents(DateTime dt)
		{
			var path = CombinePaths(Path.Combine(Constants.ContentDirectory, GetDayExtraFileName(dt)));
			if (!File.Exists(path))
				return (false, new XElement("empty"));
			var str = File.ReadAllText(path);
			return (true, XElement.Parse(str));
		}
		/// <inheritdoc cref="ITestDataProcessor.GetDayExtraValue"/>
		public void SetDayExtraValue(DateTime dt, string entryId, string key, string value)
		{
			SetValueUsingTemplate(
			  Path.Combine(Constants.ContentDirectory, GetDayExtraFileName(dt))
			  , dayExtraTransform, entryId, key, value);
		}
		/// <param name="filePathRelativeToEnvironment">e.g. contents/2018-2-8.dayentry.xml</param>
		/// <returns>e.g. c:/projects/dasblog/.../Environments/Vanilla/Contents/2108-2-8.dayentry.xml</returns>
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
		public static string GetBlogEntryFileName(DateTime blogPostDate)
		{
			return $"{blogPostDate.Year}-{blogPostDate.Month:D2}-{blogPostDate.Day:D2}.dayentry.xml";
		}
		public static string GetDayExtraFileName(DateTime blogPostDate)
		{
			return $"{blogPostDate.Year}-{blogPostDate.Month:D2}-{blogPostDate.Day:D2}.dayfeedback.xml";
		}

		public static string GetBlogFeedbackFileName(DateTime blogPostDate)
		{
			return GetDayExtraFileName(blogPostDate);
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
