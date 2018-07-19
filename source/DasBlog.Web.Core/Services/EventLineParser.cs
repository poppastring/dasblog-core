using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
				,FomratMessage(parseParts[MESSAGE])
				,FormatDate(parseParts[DATE])
			);
			return (true, eddi);
		}
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

		private string FomratMessage(string message)
		{
			return $"<span>{message}<span>";
		}

		private EventCodes FormatEventCode(string eventCode)
		{
			return Enum.Parse<EventCodes>(eventCode);
		}

	}
}