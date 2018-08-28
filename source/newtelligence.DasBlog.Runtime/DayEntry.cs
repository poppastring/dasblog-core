#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion


using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime.Proxies;
using newtelligence.DasBlog.Util;
using System.Security.Principal;
using NodaTime;

namespace newtelligence.DasBlog.Runtime
{
	[Serializable]
	[XmlRoot(Namespace=Data.NamespaceURI)]
	[XmlType(Namespace=Data.NamespaceURI)]
	public class DayEntry : IDayEntry
	{
		private object entriesLock = new object(); //the entries collection is shared and must be protected

		private bool Loaded
		{
			[DebuggerStepThrough()] get { return _loaded; }
			[DebuggerStepThrough()] set { _loaded = value; }
		}
		private bool _loaded = false;
        
		public string FileName 
		{ 
			get
			{
				// Use Invariant Culture, not host culture (or user override), for date formats.
				return DateUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".dayentry.xml";
			} 
		}

		[XmlIgnore]
		public DateTime DateUtc 
		{ 
			[DebuggerStepThrough()]	get { return _date; } 
			[DebuggerStepThrough()]	set { _date = value.Date; } 
		}
		private DateTime _date;

		[XmlElement("Date")]
		public DateTime DateLocalTime 
		{ 
			get { return (DateUtc==DateTime.MinValue||DateUtc==DateTime.MaxValue)?DateUtc:DateUtc.ToLocalTime(); } 
			set { DateUtc = (value==DateTime.MinValue||value==DateTime.MaxValue)?value:value.Date.ToUniversalTime(); 																								 } 
		}

		[XmlArrayItem(typeof(Entry))]
		public EntryCollection Entries 
		{ 
			[DebuggerStepThrough()]	get { return _entries; } 
			[DebuggerStepThrough()]	set { _entries = value; }
		}
		private EntryCollection _entries = new EntryCollection();

		[XmlAnyElement]
		public XmlElement[] anyElements;
		[XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;

		public void Initialize()
		{
			DateUtc = DateTime.Now.ToUniversalTime().Date;
		}

		/// <summary>
		/// Return EntryCollection excluding the private entries if the caller
		/// is not in the admin role.
		/// </summary>
		public EntryCollection GetEntries()
		{
			return GetEntries(null);
		}

        /// <summary>
        /// Return EntryCollection with the number of entries limited by <see paramref="maxResults" />  
        /// excluding the private entries if the caller is not in the admin role.
        /// </summary>
        public EntryCollection GetEntries(int maxResults)
        {
            return GetEntries(null, maxResults);
        }

        /// <summary>
        /// Returns the entries that meet the include delegates criteria.
        /// </summary>
        /// <param name="include">The delegate indicating which items to include.</param>
        public EntryCollection GetEntries(Predicate<Entry> include)
        {
			return GetEntries(include, Int32.MaxValue);
        }


		/// <summary>
        /// Returns the entries that meet the include delegates criteria, 
        /// with the number of entries limited by <see paramref="maxResults" />.
		/// </summary>
		/// <param name="include">The delegate indicating which items to include.</param>
		public EntryCollection GetEntries(Predicate<Entry> include, int maxResults)
		{
			lock(entriesLock)
			{
				Predicate<Entry> filter = null;

                if (System.Threading.Thread.CurrentPrincipal != null)
                {
                    if (!System.Threading.Thread.CurrentPrincipal.IsInRole("admin"))
                    {
                        filter += EntryCollectionFilter.DefaultFilters.IsPublic();
                    }
                }
                else
                {
                    filter += EntryCollectionFilter.DefaultFilters.IsPublic();
                }

				if(include != null)
				{
					filter += include;
				}

				return EntryCollectionFilter.FindAll(Entries, filter, maxResults);
			}
		}

		/// <param name="entryTitle">An URL-encoded entry title</param>
		public Entry GetEntryByTitle(string entryTitle)
		{
			foreach (Entry entry in this.Entries)
			{
				string compressedTitle = entry.CompressedTitle.Replace("+", "");

				if (CaseInsensitiveComparer.Default.Compare(compressedTitle,entryTitle) == 0)
				{
					return entry;
				}
			}

			return null;
		}

		internal void Load(DataManager data)
		{
			if ( Loaded ) 
			{
				return;
			}

			lock(entriesLock)
			{
				if ( Loaded ) //SDH: standard thread-safe double check
				{
					return;
				}
				
				string fullPath = data.ResolvePath(FileName);
				FileStream fileStream = FileUtils.OpenForRead(fullPath);
				if ( fileStream != null )
				{
					try
					{
						XmlSerializer ser = new XmlSerializer(typeof(DayEntry),Data.NamespaceURI);
						using (StreamReader reader = new StreamReader(fileStream))
						{
							//XmlNamespaceUpgradeReader upg = new XmlNamespaceUpgradeReader( reader, "", Data.NamespaceURI );
                            DayEntry e = (DayEntry)ser.Deserialize(reader);
							Entries = e.Entries;
						}
					}
					catch(Exception e)
					{
						ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
					}
					finally
					{
						fileStream.Close();
					}
				}

				Entries.Sort((left,right) => right.CreatedUtc.CompareTo(left.CreatedUtc));
			
				Loaded = true;
			}
		}
		
		internal void Save(DataManager data)
		{
			string fullPath = data.ResolvePath(FileName);

			// We use the internal list to circumvent ignoring 
			// items where IsPublic is set to false.
			if ( Entries.Count == 0 )
			{
				if ( File.Exists( fullPath ) )
				{
					File.Delete( fullPath );
				}
			}
			else
			{
                // TODO: Web Core compatability issues ???
                // System.Security.Principal.WindowsImpersonationContext wi = Impersonation.Impersonate();

                FileStream fileStream = FileUtils.OpenForWrite(fullPath);

				if ( fileStream != null )
				{
					try
					{
						XmlSerializer ser = new XmlSerializer(typeof(DayEntry),Data.NamespaceURI);
						using (StreamWriter writer = new StreamWriter(fileStream))
						{
							ser.Serialize(writer, this);
						}
					}
					catch(Exception e)
					{
						ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
					}
					finally
					{
						fileStream.Close();
					}
				}

				// wi.Undo();
			}
		}

		/// <summary>
		/// Returns true if the specified DayEntry occurs before the day specified.
		/// </summary>
		/// <param name="dayEntry">The DayEntry to check the date of.</param>
		/// <param name="dateTime">The date the DayEntry should occur before</param>
		/// <returns>Returns true if the dayEntry occurs before the specified date.</returns>
		public static bool OccursBefore(DayEntry dayEntry, DateTime dateTime)
		{
			return (dayEntry.DateUtc.Date <= dateTime);
		}

		public static bool OccursBetween(DayEntry dayEntry, DateTimeZone timeZone, 
			DateTime startDateTime, DateTime endDateTime)
		{
			//return ((timeZone.ToLocalTime(dayEntry.DateUtc) >= startDateTime)
			//	&& (timeZone.ToLocalTime(dayEntry.DateUtc) <= endDateTime) );
			return ((dayEntry.DateUtc >= startDateTime)
				&& (dayEntry.DateUtc <= endDateTime) );
		}

		/// <summary>
		/// Returns true if the specified DayEntry is within the same month as <c>month</c>;
		/// </summary>
		/// <param name="dayEntry"></param>
		/// <param name="timeZone"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static bool OccursInMonth(DayEntry dayEntry, DateTimeZone timeZone, DateTime month)
		{
			DateTime startOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0);
			DateTime endOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0);
			endOfMonth = endOfMonth.AddMonths(1);
			endOfMonth = endOfMonth.AddSeconds(-1);

			var localTime = LocalDateTime.FromDateTime(endOfMonth);
			var offset = timeZone.GetUtcOffset(localTime.InZoneStrictly(timeZone).ToInstant());
			endOfMonth = endOfMonth.Add(offset.ToTimeSpan());
			return (OccursBetween(dayEntry, timeZone, startOfMonth, endOfMonth));
		}
	}
}
