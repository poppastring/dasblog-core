using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public interface ITestDataProcessor
	{
		/// <summary>
		/// get values from config and entry files
		/// </summary>
		/// <param name="filePathRelativeToEnvironment">
		///   e.g. "Config/site.config" or "Content/2018-08-03.dayentry.xml"</param>
		/// <param name="xPath">e.g. "/dasb:DayEntry/dasb:Entries/dasb:Entry[dasb:Title='abc']/dasb:EntryId'"
		/// ****** CAUTION ******
		/// Callers must prefix all the element names with 'dasb:' because the C# XPath mechanism
		/// insist that even the default namespace is specified in the xpath</param>
		/// <returns>if operation fails then value is null otherwise it is the value identified by xPath</returns>
		(bool success, string value) GetValue(string filePathRelativeToEnvironment, string xPath);
		/// <param name="key">e.g. "LogDir"</param>
		/// <returns>e.g. "/logs/", if operation fails then value is null otherwise it is the value identified by key</returns>
		(bool success, string value) GetSiteConfigValue(string key);
		/// <param name="email">"myemail@email.com"</param>
		/// <param name="key">e.g. "Role"</param>
		/// <returns>e.g. "admin", if operation fails then value is null otherwise it is the value identified by key</returns>
		(bool success, string value) GetSiteSecurityConfigValue(string email, string key);
		// TODO: what is the behaviour for an empty file?
		/// <param name="dt">The date of the blog entry - should match the date in the file name</param>
		/// <param name="entryId">typically a GUID</param>
		/// <param name="key">e.g. "IsPublic"</param>
		/// <returns>e.g. "false", if operation fails then value is null otherwise it is the value identified by key</returns>
		(bool success, string value) GetBlogPostValue(DateTime dt, string entryId, string key);

		/// <summary>
		/// gets the complete contents of the dayentry xml file for the specified date
		/// </summary>
		/// <param name="dt">should match the date stamp on a dayentry file</param>
		/// <returns>success=true, if the file exists otherwise false
		///   data=linq ready tree starting at "DayEntry", immediate children "Date" and "Entries"</returns>
		(bool success, XElement data) GetBlogPostFileContents(DateTime dt);
/*
		/// <summary>
		/// set values in config and entry files
		/// Either replaces or inserts the value
		/// </summary>
		/// <param name="filePathRelativeToEnvironment">
		///   e.g. "Config/site.config" or "Content/2018-08-03.dayentry.xml"</param>
		/// <param name="pathPartsAndValue">e.g. ["myemail@email.com", "Role", "Contributor"]
		///   These parameters are substituted into the appropriate xxlt template</param>
		/// <exception cref="Exception">throown if the operation fails.  Message provides reason</exception>
		void SetValue(string filePathRelativeToEnvironment, params object[] pathPartsAndValue);
*/
		/// <param name="key">e.g. "LogDir"</param>
		/// <param name="value">.e.g "alternate_logs"</param>
		/// <exception cref="Exception">throown if the operation fails.  Message provides reason</exception>
		void SetSiteConfigValue(string key, string value);
		/// <param name="email">"myemail@email.com"</param>
		/// <param name="key">e.g. "Role"</param>
		/// <param name="value">e.g. "contributor"</param>
		/// <exception cref="Exception">throown if the operation fails.  Message provides reason</exception>
		void SetSiteSecurityConfigValue(string email, string key, string value);
		/// <param name="dt">The date of the blog entry - should match the date in the file name</param>
		/// <param name="pred">e.g. entry => entry.EntryId=="abc' </param>
		/// <param name="key">e.g. "IsPublic"</param>
		/// <param name="value">.e.g "false"</param>
		/// <exception cref="Exception">throown if the operation fails.  Message provides reason</exception>
		void SetBlogPostValue(DateTime dt, Expression<Func<Entry, bool>> pred, string key, object value);

		// TODO: what is the behaviour for an empty file?
		/// <summary>
		/// gets the complete contents of the dayfeedback xml file for the specified date
		/// </summary>
		/// <param name="dt">should match the date stamp on a dayfeedback file</param>
		/// <returns>success=true, if the file exists otherwise false
		///   data=linq ready tree starting at DayExtra, immediate children "Date" and "Comments"</returns>
		(bool success, XElement data) GetDayExtraFileContents(DateTime dt);

		/// <summary>
		/// typically used to get comments - maybe other stuff - I don't know
		/// </summary>
		/// <param name="dt">date forming part of the target filename of the dayfeedback file</param>
		/// <param name="entryId">typically a guid</param>
		/// <param name="key">e.g. IsPublic</param>
		/// <returns>e.g. "false"</returns>
		(bool success, string value) GetDayExtraValue(DateTime dt, string entryId, string key);
		/// <summary>
		/// typically used to replace values in comments
		/// </summary>
		/// <param name="dt">date forming part of the target filename of the dayfeedback file</param>
		/// <param name="entryId">typically a guid</param>
		/// <param name="key">e.g. IsPublic</param>
		/// <param name="value"></param>
		void SetDayExtraValue(DateTime dt, string entryId, string key, string value);
	}
}
