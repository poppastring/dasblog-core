using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DasBlog.Core.Services
{
	public class EventLineParser : Interfaces.IEventLineParser
	{
		private const int NUM_PARSE_PARTS = 7;	// 6 groups for the fields + 1 for the whole string
		private const string DATE = "date";
		private const string LOGGER_LEVEL = "loggerlevel";
		private const string CATEGORY = "category";
		private const string EVENT_CODE = "eventcode";
		private const string MESSAGE = "message";
		private const string URL = "url";
		
		public (bool, EventDataDisplayItem) Parse(string eventLine)
		{
			IDictionary<string, string> parseParts = ParseLine(eventLine);
			if (parseParts.Count != NUM_PARSE_PARTS)
			{
				return (false, null);	// not something for the activity report
			}

			EventDataDisplayItem eddi = new EventDataDisplayItem(
				FormatEventCode(parseParts[EVENT_CODE])
				,FomratMessage(parseParts[MESSAGE], parseParts[URL])
				,FormatDate(parseParts[DATE])
			);
			return (true, eddi);
		}
		/// <summary>
		/// transforms eventline into a dictionary of key value pairs as eg'd below
		/// </summary>
		/// <param name="eventLine">2018-07-19 12:57:39.719 +01:00 [Information] DasBlog.Managers.BlogManager: EntryAdded :: hopefully some logging will happen :: http://localhost:50431/hopefully-some-logging-will-happen</param>
		/// <returns>
		/// {
		///   date="2018-07-19 12:57:39.719 +01:00"
		///   ,loggerlevel="Information"
		///   ,category="DasBlog.Managers.BlogManager"
		///   ,eventcode="EntryAdded"
		///   ,message="hopefully some logging will happen"
		///   ,url="http://localhost:50431/hopefully-some-logging-will-happen"
		/// }
		/// </returns>
		private IDictionary<string, string> ParseLine(string eventLine)
		{
			Regex regex = new Regex($@"
				^
				(?<{DATE}>[^\[]*)
				\[
				(?<{LOGGER_LEVEL}>[^\]]*)
				\]
				[ ]
				(?<{CATEGORY}>[^:]+)
				:
				(?<{EVENT_CODE}>[^:]+)
				::
				(?<{MESSAGE}>.+)
				::
				(?<{URL}>.+)
				$
				"
				, RegexOptions.None | RegexOptions.IgnorePatternWhitespace);
			Match match = regex.Match(eventLine);
			var parseParts = match.Groups.Where(g => !string.IsNullOrWhiteSpace(g.Name))
				.ToDictionary(g => g.Name, g => g.Value.Trim());
			return parseParts;
		}
		private DateTime FormatDate(string date)
		{
			return DateTime.Parse(date);
		}

		private string FomratMessage(string message, string url)
		{
			return $"<span>{message} <span>"
			  + (url == null ? string.Empty
			  : $"<span><a href={url}>permalink</a></span>");
		}

		private EventCodes FormatEventCode(string eventCode)
		{
			return Enum.Parse<EventCodes>(eventCode);
		}

	}
}
