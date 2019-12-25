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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DasBlog.Core.Common.Comments
{
	/// <summary>
	/// Converts a match collection to a tag collection. 
	/// Takes care of illegal tags, non-closed, and non-balanced end-tags.
	/// </summary>
	public class MatchedTagCollection : ICollection, IEnumerable
	{
		public MatchedTagCollection(ValidCommentTags allowedTags)
		{

			// param validation
			if (allowedTags == null) { throw new ArgumentNullException("allowedTags"); }

			this.allowedTags = allowedTags;
		}

		/// <summary>
		/// Adds the tag matches in an input string.
		/// </summary>
		/// <param name="matches">The tag matches from an input string.</param>
		public void Init(MatchCollection matches)
		{

			// param validation
			if (matches == null) { throw new ArgumentNullException("matches"); }

			// holds the tags in the order they were matched
			store = new ArrayList(matches.Count);
			// pre-fill
			for (int i = 0; i < matches.Count; i++)
			{
				store.Add(null);
			}

			// holds the index of the matches that have not yet been matched
			Stack needMatching = new Stack();

			for (int i = 0; i < matches.Count; i++)
			{

				string name = matches[i].Groups["name"].Value;
				if (!allowedTags.IsValidTag(name))
				{
					// illegal tag
					store[i] = new MatchedTag(matches[i], false);
					continue;
				}

			
				var validTag = allowedTags.Tags.Single(s => s.Name == name);

				// valid match
				// if its a self-closing tag, add to store
				bool self = matches[i].Groups["self"].Value.Length > 0;
				if (self)
				{

					// this is the tag we use to write out the tag later
					MatchedTag matchedTag = new MatchedTag(matches[i], true);
					// check for invalid attributes
					matchedTag.FilterAttributes(validTag);
					// add to store
					store[i] = matchedTag;

					continue;
				}

				// end tag
				bool end = matches[i].Groups["end"].Value.Length > 0;
				if (end)
				{
					if (needMatching.Count == 0)
					{
						// no opening tags for this closing tag, marks as invalid and go to the next
						store[i] = new MatchedTag(matches[i], false);
						continue;
					}

					// usually the opening tag will be followed by the closing tag
					int peek = (int)needMatching.Peek();
					if (String.Compare(matches[peek].Groups["name"].Value, name, true, CultureInfo.InvariantCulture) == 0)
					{
						// we have a match, add both to the store
						store[i] = new MatchedTag(matches[i], true);

						// filter the attr. for the opening tag
						MatchedTag matchedTag = new MatchedTag(matches[peek], true);
						matchedTag.FilterAttributes(validTag);

						// add to store
						store[peek] = matchedTag;
						// remove from the queue
						needMatching.Pop();
						continue;
					}

					// enumerate through the stack to see if we can find a matching 
					// opening tag
					int foundIndex = -1;
					foreach (int j in needMatching)
					{
						// found a match
						if (String.Compare(matches[j].Groups["name"].Value, name, true, CultureInfo.InvariantCulture) == 0)
						{
							foundIndex = j;
							break;
						}
					}

					if (foundIndex > -1)
					{
						while (needMatching.Count > 0 && (int)needMatching.Peek() >= foundIndex)
						{
							int pop = (int)needMatching.Pop();
							if (pop == foundIndex)
							{
								// we have a match, add both to the store
								store[i] = new MatchedTag(matches[i], true);

								// filter the attr. for the opening tag
								MatchedTag matchedTag = new MatchedTag(matches[pop], true);
								matchedTag.FilterAttributes(validTag);

								// add to store
								store[pop] = matchedTag;
							}
							else
							{
								// this tag needs to be closed, since its between our opening and closing tags
								store[pop] = new MatchedTag(matches[pop], true, true);
							}
						}
					}
					else
					{
						// nothing we could do with this tag, mark as invalid
						store[i] = new MatchedTag(matches[i], false);
					}
					continue;
				}

				// opening tag add to queue until we find the matchin closing tag
				needMatching.Push(i);
			}

			// we have unmatched valid opening tags, make them self closing
			if (needMatching.Count > 0)
			{
				foreach (int i in needMatching)
				{
					// filter the attr. for the opening tag
					MatchedTag matchedTag = new MatchedTag(matches[i], true, true);

					matchedTag.FilterAttributes(allowedTags.Tags.Single(s => s.Name == matchedTag.TagName));

					// add to store
					store[i] = matchedTag;
				}
			}
		}

		/// <summary>
		/// Copies the Matchedtags to the supplied array at the specified index.
		/// </summary>
		/// <param name="array">The array to copy the tags to.</param>
		/// <param name="index">The start index.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			store.CopyTo(array, index);
		}



		/// <summary>
		/// Returns an enumerator that can iterate through the collection of <see cref="MatchedTag" />s.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/>
		/// that can be used to iterate through the collection of <see cref="MatchedTag" />s.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			if (store == null)
			{
				store = new ArrayList();
			}
			return store.GetEnumerator();
		}


		/// <summary>
		/// Gets the number of
		/// elements contained in the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value>The number of elements.</value>
		public int Count
		{
			[DebuggerStepThrough]
			get
			{
				return store.Count;
			}
		}

		/// <summary>
		/// Gets a value
		/// indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized
		/// (thread-safe).
		/// </summary>
		/// <value>Always returns <see langword="false" />.</value>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets an object that
		/// can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value>An object to synchronize access to the collection.</value>
		object ICollection.SyncRoot
		{
			get
			{
				return store.SyncRoot;
			}
		}

		// FIELDS

		ValidCommentTags allowedTags;
		ArrayList store;
	}
}
