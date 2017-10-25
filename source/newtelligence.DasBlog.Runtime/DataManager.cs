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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace newtelligence.DasBlog.Runtime
{
	internal delegate string ResolveFileCallback(string file);

	internal class DataManager
	{
//		private static BlogCoreData coreData = null;

		private object syncDays = new Object();
		private object syncCore = new Object();
		private DayEntryCollection days;

		private long _entryChangeCount;
		private long _extraChangeCount;

		internal DateTime lastEntryUpdate = DateTime.MinValue;
		internal DateTime lastCommentUpdate = DateTime.MinValue;

		public ResolveFileCallback Resolver = null;

		public string ResolvePath(string file)
		{
			return Resolver(file);
		}

        protected class DaySorter : IComparer<DayEntry>
        {
            public int Compare(DayEntry left, DayEntry right)
            {
                return right.DateUtc.CompareTo(left.DateUtc);
            }
        }

		public DataManager()
		{
		}

		public DayEntryCollection Days
		{
			get
			{
				lock (this.syncDays)
				{
					if (this.days == null)
					{
						// Use Invariant Culture, not host culture (or user override), for date formats.

						DayEntryCollection newEntries = new DayEntryCollection();
						foreach (FileInfo fi in new DirectoryInfo(ResolvePath(".")).GetFiles("*.dayentry.xml"))
						{
							string fileName = fi.Name;
							DayEntry day = new DayEntry();

							// Ignore local DateFormatInfo (could say CCYY-DD-MM), always use CCYY-MM-DD.  #000004
							day.DateUtc = DateTime.Parse(fileName.Substring(0, fileName.Length - ".dayentry.xml".Length), CultureInfo.InvariantCulture);
							newEntries.Add(day);

							// OmarS: since we no longer have blogdata.xml to store the last
							// date we added an entry, we can update this by loading the most recent
							// day that we have stored.
							if (this.lastEntryUpdate < fi.LastWriteTimeUtc)
								this.lastEntryUpdate = fi.LastWriteTimeUtc;
						}
						newEntries.Sort(new DaySorter());
						this.days = newEntries;
					}

					return new DayEntryCollection(days);  //a shallow copy
				}
			}
		}

		public DayExtra GetDayExtra(DateTime day)
		{
			DayExtra d = new DayExtra();
			d.DateUtc = day;
			d.Load(this);
			return d;
		}

		public long CurrentEntryChangeCount
		{
			get
			{
				return _entryChangeCount;
			}
		}

		public long CurrentExtraChangeCount
		{
			get
			{
				return _extraChangeCount;
			}
		}

		public void IncrementEntryChange()
		{
			lock (syncDays)
			{
				this.days = null;
			}

			lock (syncCore)
			{
				_entryChangeCount++;
			}
		}

		public void IncrementExtraChange()
		{
			lock (syncCore)
			{
				_extraChangeCount++;
			}
		}

	}
}