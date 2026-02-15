using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DasBlog.Managers
{
	public class SearchManager : ISearchManager
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IBlogDataService dataService;
		private static readonly Regex stripTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public SearchManager(IDasBlogSettings dasBlogSettings, IBlogDataService dataService)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.dataService = dataService;
		}

		public EntryCollection SearchEntries(string searchString, string acceptLanguageHeader)
		{
			var searchWords = GetSearchWords(searchString);

			var entries = dataService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), 
											dasBlogSettings.GetConfiguredTimeZone(), 
											acceptLanguageHeader, int.MaxValue, int.MaxValue, null);

			// no search term provided, return all the results
			if (searchWords.Count == 0) return entries;

			EntryCollection matchEntries = new EntryCollection();

			foreach (Entry entry in entries)
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
			/*
						ILoggingDataService logService = requestPage.LoggingService;
						string referrer = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : Request.ServerVariables["REMOTE_ADDR"];	
						logger.LogInformation(
							new EventDataItem(EventCodes.Search, String.Format("{0}", searchString), referrer));
			*/

			return matchEntries;
		}

		private static StringCollection GetSearchWords(string searchString)
		{
			var searchWords = new StringCollection();

			if (string.IsNullOrWhiteSpace(searchString))
				return searchWords;

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

			return searchWords;
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
