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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using newtelligence.DasBlog.Util.Zip;

//using newtelligence.DasBlog.Runtime.Proxies;
//using newtelligence.DasBlog.Util.Zip;

namespace newtelligence.DasBlog.Runtime
{
	internal delegate string LogDataItemFormatter(LogDataItem logItem);

	internal delegate string EventDataItemFormatter(EventDataItem logItem);

	internal class LoggingDataServiceXml : ILoggingDataService
	{
		static readonly Regex _eventDataItemParser =
			new Regex(@"l2\ttime\t(?<time>.*)\tcode\t(?<code>.*)\tmessage\t(?<message>.*)", RegexOptions.Compiled);

		static readonly Regex _logDataItemParser =
			new Regex(
				@"(c1|e2)\ttime\t(?<time>.*)\turl\t(?<url>.*)\turlreferrer\t(?<urlreferrer>.*)\tuseragent\t(?<useragent>[^\t\n]*)(\tuserdomain\t(?<userdomain>.*))?",
				RegexOptions.Compiled);

		readonly ReaderWriterLock _aggregatorBugLock = new ReaderWriterLock();
		readonly ReaderWriterLock _clickThroughLock = new ReaderWriterLock();
		readonly ReaderWriterLock _crosspostLock = new ReaderWriterLock();
		readonly ReaderWriterLock _eventLock = new ReaderWriterLock();
		readonly string _logDirectory;
		readonly ReaderWriterLock _referrerLock = new ReaderWriterLock();

		internal LoggingDataServiceXml(string loggingLocation)
		{
			_logDirectory = loggingLocation;
			if (!Directory.Exists(_logDirectory))
			{
				throw new ArgumentException(String.Format("Invalid directory {0}", _logDirectory), "loggingLocation");
			}
		}

		#region ILoggingDataService Members
		LogDataItemCollection ILoggingDataService.GetReferralsForDay(DateTime dateUtc)
		{
			string logText = GetReferrerLogText(dateUtc);
			return ParseLogDataItems(logText);
		}

		LogDataItemCollection ILoggingDataService.GetClickThroughsForDay(DateTime dateUtc)
		{
			string logText = GetClickThroughLogText(dateUtc);
			return ParseLogDataItems(logText);
		}

		LogDataItemCollection ILoggingDataService.GetAggregatorBugHitsForDay(DateTime dateUtc)
		{
			string logText = GetAggregatorBugHitLogText(dateUtc);
			return ParseLogDataItems(logText);
		}

		LogDataItemCollection ILoggingDataService.GetCrosspostReferrersForDay(DateTime dateUtc)
		{
			string logText = GetCrosspostReferrerLogText(dateUtc);
			return ParseLogDataItems(logText);
		}

		EventDataItemCollection ILoggingDataService.GetEventsForDay(DateTime dateUtc)
		{
			string logText = GetEventLogText(dateUtc);
			return ParseEventDataItems(logText);
		}

		void ILoggingDataService.AddReferral(LogDataItem logItem)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AddLogDataItemWorker),
										 new WriterThreadParams<LogDataItem>(logItem, LogCategory.Referrer, _referrerLock));
		}

		void ILoggingDataService.AddClickThrough(LogDataItem logItem)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AddLogDataItemWorker),
			                             new WriterThreadParams<LogDataItem>(logItem, LogCategory.ClickThrough,
			                                                                 _clickThroughLock));
		}

		void ILoggingDataService.AddAggregatorBugHit(LogDataItem logItem)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AddLogDataItemWorker),
			                             new WriterThreadParams<LogDataItem>(logItem, LogCategory.AggregatorBug,
			                                                                 _aggregatorBugLock));
		}

		void ILoggingDataService.AddCrosspostReferrer(LogDataItem logItem)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AddLogDataItemWorker),
			                             new WriterThreadParams<LogDataItem>(logItem, LogCategory.Crosspost,
			                                                                 _crosspostLock));
		}

		void ILoggingDataService.AddEvent(EventDataItem eventData)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AddEventDataItemWorker),
			                             new WriterThreadParams<EventDataItem>(eventData, LogCategory.Event, _eventLock));
		}

		#region Log Text Parsing
		static LogDataItemCollection ParseLogDataItems(string logText)
		{
			LogDataItemCollection result = new LogDataItemCollection();

			MatchCollection matches = _logDataItemParser.Matches(logText);
			if (matches != null)
			{
				foreach (Match match in matches)
				{
					LogDataItem item = new LogDataItem();
					item.RequestedUtc = DateTime.Parse(match.Groups["time"].Value);
					item.UrlRequested = match.Groups["url"].Value;
					item.UrlReferrer = match.Groups["urlreferrer"].Value;
					item.UserAgent = match.Groups["useragent"].Value;
					item.UserDomain = match.Groups["userdomain"].Value;
					result.Add(item);
				}
			}

			return result;
		}

		static EventDataItemCollection ParseEventDataItems(string logText)
		{
			EventDataItemCollection result = new EventDataItemCollection();

			MatchCollection matches = _eventDataItemParser.Matches(logText);
			if (matches != null)
			{
				foreach (Match match in matches)
				{
					EventDataItem item = new EventDataItem();
					item.EventTimeUtc = DateTime.Parse(match.Groups["time"].Value);
					item.EventCode = Int32.Parse(match.Groups["code"].Value);
					item.HtmlMessage = match.Groups["message"].Value;
					result.Add(item);
				}
			}

			return result;
		}
		#endregion

		#region Log File Archiving

		void ArchiveLogFiles(DateTime dt)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(ArchiveLogFileWorker),
			                             new ArchiveThreadParams(GetLogPath(dt, LogCategory.Event), _eventLock));
			ThreadPool.QueueUserWorkItem(new WaitCallback(ArchiveLogFileWorker),
			                             new ArchiveThreadParams(GetLogPath(dt, LogCategory.Referrer), _referrerLock));
			ThreadPool.QueueUserWorkItem(new WaitCallback(ArchiveLogFileWorker),
			                             new ArchiveThreadParams(GetLogPath(dt, LogCategory.ClickThrough), _clickThroughLock));
			ThreadPool.QueueUserWorkItem(new WaitCallback(ArchiveLogFileWorker),
			                             new ArchiveThreadParams(GetLogPath(dt, LogCategory.AggregatorBug), _aggregatorBugLock));
			ThreadPool.QueueUserWorkItem(new WaitCallback(ArchiveLogFileWorker),
			                             new ArchiveThreadParams(GetLogPath(dt, LogCategory.Crosspost), _crosspostLock));
		}

		static void ArchiveLogFileWorker(object argument)
		{
			ArchiveThreadParams param = (ArchiveThreadParams) argument;

            string zipFileName = null;
            bool deletedOriginal = false;

			try
			{
				if (File.Exists(param.FileName))
				{
					param.LockObject.AcquireReaderLock(TimeSpan.FromMilliseconds(250));

                    zipFileName = String.Format("{0}.zip", param.FileName);

                    // properly dispose when we're done
                    using (ZipFile zip = new ZipFile(zipFileName)) {
                        // don't include the dir tree in the zipfile
                        zip.IncludeDirectoryTree = false;
                        zip.AddFile(param.FileName);
                        zip.Save();
                    }
    
					File.Delete(param.FileName);
                    deletedOriginal = true;
				}
			}
			catch (Exception e)
			{

                ErrorTrace.Trace(TraceLevel.Error, e);

                // error while creating the zipfile, delete it so we can create a new one later
                if (zipFileName !=null && File.Exists(zipFileName) && !deletedOriginal) {
                    File.Delete(zipFileName);
                }

                // logging
                //while (e != null) {
                //    File.AppendAllText(param.FileName + ".error.txt", "----------------------------------------------------------------" + "\r\n");
                //    File.AppendAllText(param.FileName + ".error.txt", "filename: " + param.FileName + "\r\n");
                //    File.AppendAllText(param.FileName + ".error.txt", e.Message + "\r\n");
                //    File.AppendAllText(param.FileName + ".error.txt", e.StackTrace + "\r\n");
                //    e = e.InnerException;
                //}
            
            }
			finally
			{
				if (param.LockObject.IsReaderLockHeld)
				{
					param.LockObject.ReleaseReaderLock();
				}
			}
		}
		#endregion

		#region Log File Read Routines
		string GetLogPath(DateTime dateUtc, LogCategory category)
		{
			string fileNameSuffix;

			switch (category)
			{
				case LogCategory.Event:
					fileNameSuffix = "events";
					break;

				case LogCategory.AggregatorBug:
					fileNameSuffix = "aggbug";
					break;

				case LogCategory.ClickThrough:
					fileNameSuffix = "clickthrough";
					break;

				case LogCategory.Crosspost:
					fileNameSuffix = "crosspost";
					break;

				case LogCategory.Referrer:
					fileNameSuffix = "referrer";
					break;

				default:
					throw new ArgumentOutOfRangeException("category", category, "Unknown log category.");
			}

			return Path.Combine(_logDirectory, String.Format("{0:yyyy-MM-dd}.{1}.log", dateUtc.Date, fileNameSuffix));
		}

		string GetLogText(DateTime dateUtc, LogCategory category, ReaderWriterLock lockObject)
		{
			string result = String.Empty;

			try
			{
				// Check for the zip version first.
				string logFile = String.Format("{0}.zip", GetLogPath(dateUtc, category));
				if (!File.Exists(logFile))
				{
					logFile = GetLogPath(dateUtc, category);
				}

				result = ReadLogText(logFile, lockObject);
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}

			return result;
		}

		static string ReadLogText(string logFilePath, ReaderWriterLock lockObject)
		{
			if (String.IsNullOrEmpty(logFilePath))
			{
				throw new ArgumentOutOfRangeException("logFilePath");
			}

			if (lockObject == null)
			{
				throw new ArgumentNullException("lockObject");
			}

			string result = String.Empty;

			try
			{
				lockObject.AcquireReaderLock(TimeSpan.FromMilliseconds(250));

				if (File.Exists(logFilePath) &&
				    Path.GetExtension(logFilePath).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
				{
					ZipFile zip = ZipFile.Read(logFilePath);
					foreach (ZipEntry e in zip)
					{
						using (MemoryStream m = new MemoryStream())
						{
							e.Extract(m);
							using (StreamReader reader = new StreamReader(m, Encoding.UTF8))
							{
								result = reader.ReadToEnd();
							}
						}
					}
				}
				else
				{
					using (StreamReader reader = new StreamReader(logFilePath))
					{
						result = reader.ReadToEnd();
					}
				}
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}
			finally
			{
				if (lockObject.IsReaderLockHeld)
				{
					lockObject.ReleaseReaderLock();
				}
			}

			return result;
		}
		#endregion

		#region Get<Category>LogText
		string GetReferrerLogText(DateTime dateUtc)
		{
			return GetLogText(dateUtc, LogCategory.Referrer, _referrerLock);
		}

		string GetClickThroughLogText(DateTime dateUtc)
		{
			return GetLogText(dateUtc, LogCategory.ClickThrough, _clickThroughLock);
		}

		string GetAggregatorBugHitLogText(DateTime dateUtc)
		{
			return GetLogText(dateUtc, LogCategory.AggregatorBug, _aggregatorBugLock);
		}

		string GetCrosspostReferrerLogText(DateTime dateUtc)
		{
			return GetLogText(dateUtc, LogCategory.Crosspost, _crosspostLock);
		}

		string GetEventLogText(DateTime dateUtc)
		{
			return GetLogText(dateUtc, LogCategory.Event, _eventLock);
		}
		#endregion

		#region Write LogDataItem and EventDataItem
		static string DefaultLogDataItemFormatter(LogDataItem logItem)
		{
			return String.Format("e2\ttime\t{0:s}\turl\t{1}\turlreferrer\t{2}\tuseragent\t{3}\tuserdomain\t{4}",
			                     logItem.RequestedUtc,
			                     logItem.UrlRequested,
			                     logItem.UrlReferrer,
			                     logItem.UserAgent,
			                     logItem.UserDomain);
		}

		static string DefaultEventDataItemFormatter(EventDataItem logItem)
		{
			string htmlMessage = logItem.HtmlMessage.Replace("\n", "");

			return String.Format("l2\ttime\t{0:s}\tcode\t{1}\tmessage\t{2}",
			                     logItem.EventTimeUtc,
			                     logItem.EventCode,
			                     htmlMessage);
		}

		void WriteLogDataItem(LogDataItem logItem, LogCategory category, ReaderWriterLock lockObject,
		                      LogDataItemFormatter formatter)
		{
			if (logItem == null)
			{
				throw new ArgumentNullException("logItem");
			}

			if (lockObject == null)
			{
				throw new ArgumentNullException("lockObject");
			}

			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}

			try
			{
				//using (Impersonation.Impersonate())
				{
					lockObject.AcquireWriterLock(TimeSpan.FromMilliseconds(250));

					using (StreamWriter writer = new StreamWriter(GetLogPath(logItem.RequestedUtc, category), true))
					{
						writer.WriteLine(formatter(logItem));
					}
				}
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}
			finally
			{
				if (lockObject.IsWriterLockHeld)
				{
					lockObject.ReleaseWriterLock();
				}
			}
		}

		void WriteEventDataItem(EventDataItem logItem, LogCategory category, ReaderWriterLock lockObject,
		                        EventDataItemFormatter formatter)
		{
			if (logItem == null)
			{
				throw new ArgumentNullException("logItem");
			}

			if (lockObject == null)
			{
				throw new ArgumentNullException("lockObject");
			}

			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}

			try
			{
				//using (Impersonation.Impersonate())
				{
					// Archive last day's log files.
					if (!File.Exists(String.Format("{0}.zip", GetLogPath(logItem.EventTimeUtc, LogCategory.Event))))
					{
						ArchiveLogFiles(logItem.EventTimeUtc.AddDays(-1));
					}

					lockObject.AcquireWriterLock(TimeSpan.FromMilliseconds(250));

					using (StreamWriter writer = new StreamWriter(GetLogPath(logItem.EventTimeUtc, category), true))
					{
						writer.WriteLine(formatter(logItem));
					}
				}
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}
			finally
			{
				if (lockObject.IsWriterLockHeld)
				{
					lockObject.ReleaseWriterLock();
				}
			}
		}
		#endregion

		#region Add Log Item Workers
		void AddLogDataItemWorker(object argument)
		{
			try
			{
				WriterThreadParams<LogDataItem> param = (WriterThreadParams<LogDataItem>) argument;
				WriteLogDataItem(param.LogItem, param.Category, param.LockObject, DefaultLogDataItemFormatter);
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}
		}

		void AddEventDataItemWorker(object argument)
		{
			try
			{
				WriterThreadParams<EventDataItem> param = (WriterThreadParams<EventDataItem>) argument;
				WriteEventDataItem(param.LogItem, param.Category, param.LockObject, DefaultEventDataItemFormatter);
			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
			}
		}
		#endregion

		#endregion

		#region Nested type: ArchiveThreadParams
		struct ArchiveThreadParams
		{
			public readonly string FileName;
			public readonly ReaderWriterLock LockObject;

			public ArchiveThreadParams(string fileName, ReaderWriterLock lockObject)
			{
				if (String.IsNullOrEmpty(fileName))
				{
					throw new ArgumentOutOfRangeException("fileName");
				}

				if (lockObject == null)
				{
					throw new ArgumentNullException("lockObject");
				}

				FileName = fileName;
				LockObject = lockObject;
			}
		}
		#endregion

		#region Nested type: WriterThreadParams
		struct WriterThreadParams<T> where T : class
		{
			public readonly LogCategory Category;
			public readonly ReaderWriterLock LockObject;
			public readonly T LogItem;

			public WriterThreadParams(T logItem, LogCategory category, ReaderWriterLock lockObject)
			{
				if (logItem == null)
				{
					throw new ArgumentNullException("logItem");
				}

				if (lockObject == null)
				{
					throw new ArgumentNullException("lockObject");
				}

				LogItem = logItem;
				Category = category;
				LockObject = lockObject;
			}
		}
		#endregion
	}
}
