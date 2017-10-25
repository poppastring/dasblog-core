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
using System.Threading;
using System.Xml.Serialization;


namespace newtelligence.DasBlog.Runtime {
	
    /// <summary>
	/// Comments collections stored on the file system.
	/// </summary>
	/// <remarks>This class uses the allcomments.xml file in the root. The user of the class
	/// is expected to use the add and/or add range methods to (re)create the file.</remarks>
	internal sealed class CommentFile { /* : ICommentCollection */
		
		private CommentCollection _commentCache;
		private DateTime _lastUpdated = DateTime.UtcNow;

		/// <summary>
		/// Creates a new instance of the <see cref="CommentFile" /> class.
		/// </summary>
		/// <param name="contentBaseDirectory">The directory to load the content from.</param>
		public CommentFile(string contentBaseDirectory) {
			// set fields
			this.contentBaseDirectory = contentBaseDirectory;
			Rebuild();
		}

		public void Rebuild()
		{
			// get the lock
			fileLock.AcquireWriterLock(100);
			try
			{
				XmlSerializer x = new XmlSerializer(typeof(DayExtra));
				CommentCollection rebuiltCollection = new CommentCollection();
				DirectoryInfo di = new DirectoryInfo(this.contentBaseDirectory);
				foreach (FileInfo file in di.GetFiles("*dayfeedback.xml"))
				{
					using(FileStream fs = file.OpenRead())
					{
						DayExtra de = (DayExtra)x.Deserialize(fs);
						rebuiltCollection.AddRange(de.Comments);
					}
				}
				_commentCache = rebuiltCollection;
			} 
			catch(Exception e) 
			{
				// report error
				ErrorTrace.Trace(TraceLevel.Error,e);
			}
			finally
			{
				// release the lock
				fileLock.ReleaseWriterLock();
			}
		}

		// METHODS
				
		// note: simple implementation: 
		// we don't want to load the collection, remove 1 comment and save it again
		// but until we have hard numbers this is too slow, let's use this

				 
		/// <summary>
		/// Adds the comment to the all comments file.
		/// </summary>
		/// <param name="comment">The comment to add.</param>
		public void AddComment( Comment comment ){

			// parameter check
			if(comment== null){
				throw new ArgumentNullException( "comment");
			}

			// get the lock
			fileLock.AcquireWriterLock(100);
			try{
				// check for exiting comment
				if( _commentCache[comment.EntryId] != null ){
					throw new ArgumentException("Comment all ready exists in the allcomments file, use the update comment method to update.", "comment");
				}

				// add the comment
				_commentCache.Add( comment );
				_lastUpdated = DateTime.UtcNow;
			} 
			catch(Exception e) 
			{
				// report error
				ErrorTrace.Trace(TraceLevel.Error,e);
			}
			finally
			{
				// release the lock
				fileLock.ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Updates a comment in the all comments file.
		/// </summary>
		/// <param name="comment">The new version of the comment.</param>
		public void UpdateComment( Comment comment ){
			// parameter check
			if(comment== null){
				throw new ArgumentNullException( "comment");
			}

			// get the lock
			fileLock.AcquireWriterLock(100);
			try{
				// check for exiting comment
				Comment oldComment = _commentCache[comment.EntryId];

				if( oldComment  == null ){
					throw new ArgumentException("Comment does not exist in the allcomments file, use the add comment method to add a new comment.", "comment");
				}

				// replace the old comment 
				_commentCache.Remove(oldComment);
				_commentCache.Add( comment );
				_lastUpdated = DateTime.UtcNow;
			} 
			catch(Exception e) 
			{
				// report error
				ErrorTrace.Trace(TraceLevel.Error,e);
			}
			finally
			{
				// release the lock
				fileLock.ReleaseWriterLock();
			}

		}

		public void DeleteComment( string commentId ){

			if( commentId == null || commentId.Length == 0 ){
				throw new ArgumentNullException( "commentId" );
			}
			
			// get the lock
			fileLock.AcquireWriterLock(100);
			try{
				// find the comment to delete
				Comment deletedComment = _commentCache[commentId];
			
				// did we get a comment?
				if( deletedComment != null ){
					// remove from collection
					_commentCache.Remove(deletedComment );
					_lastUpdated = DateTime.UtcNow;
				}
			} 
			catch(Exception e) 
			{
				// report error
				ErrorTrace.Trace(TraceLevel.Error,e);
			}
			finally
			{
				// release the lock
				fileLock.ReleaseWriterLock();
			}

		}

		/// <summary>
		/// Loads the comment collection from the file system.
		/// </summary>
		/// <returns>A colletion of all comments on the blog.</returns>
		public CommentCollection LoadComments(){

			return _commentCache;
		}

		/// <summary>
		/// Saves the comment collection to the file system as a new file.
		/// </summary>
		/// <param name="comments">The commentcollection to save.</param>
		[Obsolete("Comments are automatically cached in memory now")]
		public void SaveComments( CommentCollection comments ){
		}

		/// <summary>
		/// Returns the last time the allcomments.xml file was writen too in UTC.
		/// </summary>
		/// <returns></returns>
		public DateTime GetLastCommentUpdate(){

			return _lastUpdated;
		}


		// some locking to prevent 2 threads from writing to the file at the same time
		// this will cause all kinds of nastiness; a static lock is visible to all threads, 
		// which makes sense since we're only protecting one file
		private static ReaderWriterLock fileLock = new ReaderWriterLock();
		
		private string contentBaseDirectory;
	}
}
