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


using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
//using newtelligence.DasBlog.Util;

namespace newtelligence.DasBlog.Runtime
{
    [XmlRoot(Namespace = Data.NamespaceURI)]
    [XmlType(Namespace = Data.NamespaceURI)]
    public class CategoryCache
    {

        internal class CategorySorter : IComparer<CategoryCacheEntry>
        {

            public int Compare(CategoryCacheEntry x, CategoryCacheEntry y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        private static long _changeNumber;
        private static bool _booting = true;
        private static CategoryCacheEntryCollection _entries;
        private static object _entriesLock = new object();
        private static Dictionary<string, string> _urlSafeCategories = new Dictionary<string, string>();

        [XmlIgnore]
        public string FileName { get { return "categoryCache.xml"; } }
        public long ChangeNumber { get { return _changeNumber; } set { _changeNumber = value; } }
        public CategoryCacheEntryCollection Entries { get { return _entries; } set { _entries = value; } }
        public Dictionary<string, string> UrlSafeCategories { get { return _urlSafeCategories; } set { _urlSafeCategories = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;

        internal void Ensure(DataManager data)
        {
            lock (_entriesLock)
            {
                Load(data);
                if (_booting || ChangeNumber != data.CurrentEntryChangeCount)
                {
                    _booting = false;
                    Build(data);
                    Save(data);
                }
            }
        }

        internal void Build(DataManager data)
        {
            ChangeNumber = data.CurrentEntryChangeCount;
            Dictionary<string,CategoryCacheEntry> build = new Dictionary<string,CategoryCacheEntry>();
            CategoryCacheEntry categoryCacheEntry;

            foreach (DayEntry day in data.Days)
            {
                day.Load(data);

                foreach (Entry entry in day.Entries)
                {
                    foreach (string cat in entry.GetSplitCategories())
                    {
                        if (!build.ContainsKey(cat))
                        {
                            categoryCacheEntry = new CategoryCacheEntry();
                            categoryCacheEntry.Name = cat;
                            categoryCacheEntry.EntryDetails = new CategoryCacheEntryDetailCollection();
                            build[cat] = categoryCacheEntry;
                            if (!_urlSafeCategories.ContainsKey(Entry.InternalCompressTitle(cat.ToLower())))
                            {
								_urlSafeCategories.Add(Entry.InternalCompressTitle(cat.ToLower()), cat);
                            }
                        }
                        else
                        {
                            categoryCacheEntry = build[cat];
                        }
                        CategoryCacheEntryDetail entryDetail = new CategoryCacheEntryDetail();
                        entryDetail.DayDateUtc = day.DateUtc;
                        entryDetail.EntryId = entry.EntryId;

                        // If we have not yet found a public entry but the
                        // current entry is public then
                        if (!categoryCacheEntry.IsPublic && entry.IsPublic)
                        {
                            categoryCacheEntry.IsPublic = true;
                        }

                        categoryCacheEntry.EntryDetails.Add(entryDetail);
                    }
                }
            }

            _entries.Clear();
            foreach (KeyValuePair<string,CategoryCacheEntry> de in build)
            {
                _entries.Add(de.Value);
            }

            _entries.Sort(new CategorySorter());
        }

        private void Load(DataManager data)
        {
            if (_entries == null)
            {
                _entries = new CategoryCacheEntryCollection();
            }
        }

        private void Save(DataManager data)
        {
            _entries.Sort(new CategorySorter());
        }
    }
}
