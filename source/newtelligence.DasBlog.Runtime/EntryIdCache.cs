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


namespace newtelligence.DasBlog.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    internal class EntryIdCache
    {

        private EntryIdCache()
        {
            // empty
        }

        // storage
        private EntryCollection entriesCache;
        private Dictionary<string, DateTime> entryIDToDate = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, DateTime> compressedTitleToDate = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

        // used for synchronisation
        private static object entriesLock = new object();
        private bool booting = true;

        // used for versioning the cache
        private long changeNumber;

        /// <summary>
        /// The CurrentEntryChangeCount from the datamanager, 
        /// when we build the cache.
        /// </summary>
        public long ChangeNumber { get { return changeNumber; } }

        // the instance
        private static EntryIdCache instance = new EntryIdCache();


        /// <summary>
        /// Gets the instance of the EntryIdCache.
        /// </summary>
        /// <param name="data">Datamanager used to load the data.</param>
        /// <returns>The instance.</returns>
        public static EntryIdCache GetInstance(DataManager data)
        {

            //  handles the thread safe loading,
            // can't be moved to the constructor,
            // because a change in the 'CurrentEntryChangeCount' will force a rebuild
            instance.Ensure(data);

            return instance;
        }

        /// <summary>
        /// Returns a collection of 'lite' entries, with the comment and the description set to String.Empty 
        /// and the attached collections cleared for faster access.
        /// </summary>
        /// <returns></returns>
        public EntryCollection GetEntries()
        {

            return (EntryCollection)entriesCache.Clone();
        }

        private void Ensure(DataManager data)
        {

            if (!Loaded || booting || changeNumber != data.CurrentEntryChangeCount)
            {
                lock (entriesLock)
                {
                    if (!Loaded || booting || changeNumber != data.CurrentEntryChangeCount)
                    {
                        if (Build(data))
                        {
                            // if we succesfully build, we're no longer booting
                            booting = false;
                        }
                    }
                }
            }
        }

        private bool Build(DataManager data)
        {

            EntryCollection entriesCacheCopy = new EntryCollection();

            Dictionary<string, DateTime> entryIdToDateCopy = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<string, DateTime> compressedTitleToDateCopy = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

            try
            {
                foreach (DayEntry day in data.Days)
                {
                    day.Load(data);

                    foreach (Entry entry in day.Entries)
                    {

                        // create a lite entry for faster searching 
                        Entry copy = entry.Clone();
                        copy.Content = "";
                        copy.Description = "";

                        copy.AttachmentsArray = null;
                        copy.CrosspostArray = null;

                        entriesCacheCopy.Add(copy);

                        entryIdToDateCopy.Add(copy.EntryId, copy.CreatedUtc.Date);

                        //SDH: Only the first title is kept, in case of duplicates
                        // TODO: should be able to fix this, but it's a bunch of work.
                        string compressedTitle = copy.CompressedTitle;
                        compressedTitle = compressedTitle.Replace("+", "");
                        if (compressedTitleToDateCopy.ContainsKey(compressedTitle) == false)
                        {
                            compressedTitleToDateCopy.Add(compressedTitle, copy.CreatedUtc.Date);
                        }
                    }
                }
            }

                //TODO: SDH: Temporary, as sometimes we get collection was modified from the EntryCollection...why does this happen? "Database" corruption? 
            // Misaligned Entries?
            catch (InvalidOperationException) //something wrong enumerating the entries?
            {
                //set flags to start over to prevent getting stuck...
                booting = true;
                changeNumber = 0;
                throw;
            }

            //try to be a little more "Atomic" as others have been enumerating this list...
            entryIDToDate.Clear();
            entryIDToDate = entryIdToDateCopy;

            compressedTitleToDate.Clear();
            compressedTitleToDate = compressedTitleToDateCopy;

            entriesCache = entriesCacheCopy;

            changeNumber = data.CurrentEntryChangeCount;

            return true;
        }

        internal string GetTitleFromEntryId(string entryid)
        {
            Entry retVal = entriesCache[entryid];
            if (retVal == null)
            {
                return null;
            }
            return retVal.Title;
        }

        internal DateTime GetDateFromEntryId(string entryid)
        {
            DateTime retVal;

            if (entryIDToDate.TryGetValue(entryid, out retVal))
            {
                return retVal;
            }

            return DateTime.MinValue;
        }

        internal DateTime GetDateFromCompressedTitle(string title)
        {
            DateTime retVal;

            if (compressedTitleToDate.TryGetValue(title, out retVal))
            {
                return retVal;
            }

            return DateTime.MinValue;
        }

        private bool Loaded
        {
            get
            {
                //return true if we are loaded
                if (entriesCache == null)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
