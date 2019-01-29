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

using newtelligence.DasBlog.Runtime.Util;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime
{
	[Serializable, XmlRoot(Namespace=Data.NamespaceURI), XmlType(Namespace=Data.NamespaceURI)]
	public class DayExtra
	{
		private CommentCollection _comments = new CommentCollection();
		private TrackingCollection _trackings = new TrackingCollection();
		private DateTime _date;
		private bool _loaded = false;
		static internal bool UpgradeFilesToUtc = true;

		public string FileName
		{
			get
			{
				// Use Invariant Culture, not host culture (or user override), for date formats.

				// we want to load the old file, but make sure that we save out the new format
				// Ignore local DateFormatInfo (could say CCYY-DD-MM), always use CCYY-MM-DD.
				if (UpgradeFilesToUtc)
					return DateUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".dayfeedback.xml";
				else
					return DateLocalTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".dayextra.xml";
			}
		}

		[XmlIgnore]
		public DateTime DateUtc
		{
			get	{ return _date;	}
			set	{ _date = value.Date; }
		}

		[XmlElement("Date")]
		public DateTime DateLocalTime
		{
			get { return (DateUtc == DateTime.MinValue || DateUtc == DateTime.MaxValue) ? DateUtc : DateUtc.ToLocalTime(); }
			set { DateUtc = (value == DateTime.MinValue || value == DateTime.MaxValue) ? value : value.Date.ToUniversalTime(); }
		}

		public CommentCollection Comments
		{
			get { return _comments; }
		}

		public TrackingCollection Trackings
		{
			get { return _trackings; }
		}

		[XmlAnyElement]
		public XmlElement[] anyElements;

		[XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;

		public DayExtra()
		{
			
		}

		public DayExtra(string filePath)
		{
			LoadDayExtra(filePath);
		}

		public void Initialize()
		{
			DateUtc = DateTime.Now.ToUniversalTime().Date;
		}

		internal CommentCollection GetCommentsFor(string entryId, DataManager data)
		{
			Load(data);
			CommentCollection filtered = new CommentCollection();

			foreach (Comment c in Comments)
			{
				if (c.TargetEntryId.ToUpper() == entryId.ToUpper())
				{
					filtered.Add(c);
				}
			}
			return filtered;
		}

		internal TrackingCollection GetTrackingsFor(string entryId, DataManager data)
		{
			Load(data);
			TrackingCollection filtered = new TrackingCollection();

			foreach (Tracking c in Trackings)
			{
				if (c.TargetEntryId.ToUpper() == entryId.ToUpper())
				{
					filtered.Add(c);
				}
			}
			return filtered;
		}

		internal void Load(DataManager data)
		{
			if (_loaded)
			{
				return;
			}

			if (data.Resolver != null)
			{
				string fullPath = data.ResolvePath(FileName);
				string oldPath = data.ResolvePath(DateLocalTime.ToString("yyyy-MM-dd") + ".dayextra.xml");
				
				if (UpgradeFilesToUtc && File.Exists(oldPath))
				{
					LoadDayExtra(oldPath);

					// backup the old file
					try
					{
						DirectoryInfo backup = new DirectoryInfo(Path.Combine(data.ResolvePath(""), "backup"));

						if (!backup.Exists)
						{
							backup.Create();
						}
						
						FileInfo f = new FileInfo(oldPath);
						f.MoveTo(Path.Combine(backup.FullName, f.Name));
					}
					catch (Exception e)
					{
						ErrorTrace.Trace(TraceLevel.Error, e);
					}

					// now write the new file
					this.Save(data);
				}
				else if (File.Exists(fullPath))
				{
					LoadDayExtra(fullPath);
				}

			}

			_loaded = true;

		}

		internal void LoadDayExtra(string fullPath)
		{
			FileStream fileStream = FileUtils.OpenForRead(fullPath);
			if (fileStream != null)
			{
				try
				{
					XmlSerializer ser = new XmlSerializer(typeof (DayExtra), Data.NamespaceURI);
					using (StreamReader reader = new StreamReader(fileStream))
					{
						//XmlNamespaceUpgradeReader upg = new XmlNamespaceUpgradeReader(reader, "", Data.NamespaceURI);
                        DayExtra e = (DayExtra)ser.Deserialize(reader);
						this._comments = e.Comments;
						this._trackings = e.Trackings;
					}
				}
				catch (Exception e)
				{
					ErrorTrace.Trace(TraceLevel.Error, e);
				}
				finally
				{
					fileStream.Close();
				}

				//RepairComments();
			}
		}

		internal void Save(DataManager data)
		{
			FileStream fileStream = FileUtils.OpenForWrite(data.ResolvePath(FileName));
			if (fileStream != null)
			{
				// WindowsImpersonationContext wi = Impersonation.Impersonate();

				try
				{
					XmlSerializer ser = new XmlSerializer(typeof (DayExtra), Data.NamespaceURI);
					using (StreamWriter writer = new StreamWriter(fileStream))
					{
						ser.Serialize(writer, this);
					}
				}
				catch (Exception e)
				{
					ErrorTrace.Trace(TraceLevel.Error, e);
					// truncate the file if this fails
					fileStream.SetLength(0);
				}
				finally
				{
					fileStream.Close();
				}

				// wi.Undo();
			}
		}
	}
}
