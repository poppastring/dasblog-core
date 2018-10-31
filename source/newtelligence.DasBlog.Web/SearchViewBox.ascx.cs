using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class SeachViewBox : UserControl
	{
		private SharedBasePage requestPage;
		protected ResourceManager resmgr;

		private static Regex stripTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		protected void Page_Init(object sender, EventArgs e)
		{
			resmgr = ApplicationResourceTable.Get();

			requestPage = Page as SharedBasePage;

			if (Request.QueryString["q"] != null && Page.IsPostBack == false)
			{
				string searchQuery = System.Web.HttpUtility.UrlDecode(Request.QueryString["q"]);
				EntryCollection entries = SearchEntries(searchQuery);

				requestPage.WeblogEntries.AddRange(entries);

				foreach (Entry entry in requestPage.WeblogEntries)
				{
					requestPage.ProcessItemTemplate(entry, contentPlaceHolder);
				}
			}
			else
			{
				requestPage.WeblogEntries.AddRange(new EntryCollection());
			}

			labelSearchQuery.Text = String.Format("{0}: {1}", resmgr.GetString("text_search_query_title"), Request.QueryString["q"]);

			DataBind();
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(this.Page_Init);
		}

		#endregion

		public EntryCollection SearchEntries(string searchString)
		{
			StringCollection searchWords = new StringCollection();

			string[] splitString = Regex.Split(searchString, @"(""[^""]*"")", RegexOptions.IgnoreCase |
				RegexOptions.Compiled);

			for (int index = 0; index < splitString.Length; index++)
			{
				if (splitString[index] != "")
				{
					if (index == splitString.Length - 1)
					{
						foreach (string s in splitString[index].Split(' '))
						{
							if (s != "") searchWords.Add(s);
						}
					}
					else
					{
						searchWords.Add(splitString[index].Substring(1, splitString[index].Length - 2));
					}
				}
			}

			EntryCollection matchEntries = new EntryCollection();

			foreach (Entry entry in requestPage.DataService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), new UTCTimeZone(), Request.Headers["Accept-Language"], int.MaxValue, int.MaxValue, null))
			{
				string entryTitle = entry.Title;
				string entryDescription = entry.Description;
				string entryContent = entry.Content;

				foreach (string searchWord in searchWords)
				{
					if (entryTitle != null)
					{
						if (searchEntryForWord(entryTitle, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
					if (entryDescription != null)
					{
						if (searchEntryForWord(entryDescription, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
					if (entryContent != null)
					{
						if (searchEntryForWord(entryContent, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
				}
			}

			// log the search to the event log
            ILoggingDataService logService = requestPage.LoggingService;
			string referrer = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : Request.ServerVariables["REMOTE_ADDR"];	
			logService.AddEvent(
				new EventDataItem(EventCodes.Search, String.Format("{0}", searchString), referrer));

			return matchEntries;
		}

		private bool searchEntryForWord(string sourceText, string searchWord)
		{
			// Remove any tags from sourceText.
			sourceText = stripTags.Replace(sourceText, String.Empty);

			CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;
			return (myComp.IndexOf(sourceText, searchWord, CompareOptions.IgnoreCase) >= 0);
		}
	}
}
