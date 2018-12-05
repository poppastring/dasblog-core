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
using System.Text;
using System.Xml.Serialization;

namespace DasBlog.Core.Configuration
{
    [Serializable]
    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class ContentFilterCollection : CollectionBase, IEnumerable<ContentFilter>
    {
        public ContentFilterCollection()
        {
            // empty
        }

        public ContentFilterCollection(ContentFilter[] items)
        {
            this.AddRange(items);
        }

        public ContentFilterCollection(ContentFilterCollection items)
        {
            this.AddRange(items);
        }

        public virtual void AddRange(ContentFilter[] items)
        {
            foreach (ContentFilter item in items)
            {
                this.List.Add(item);
            }
        }

        public virtual void AddRange(ContentFilterCollection items)
        {
            foreach (ContentFilter item in items)
            {
                this.List.Add(item);
            }
        }

        public virtual void Add(ContentFilter value)
        {
            this.List.Add(value);
        }

        public virtual bool Contains(ContentFilter value)
        {
            return this.List.Contains(value);
        }

        public virtual int IndexOf(ContentFilter value)
        {
            return this.List.IndexOf(value);
        }

        public virtual void Insert(int index, ContentFilter value)
        {
            this.List.Insert(index, value);
        }

        public virtual ContentFilter this[int index]
        {
            get { return (ContentFilter)this.List[index]; }
            set { this.List[index] = value; }
        }

        public virtual void Remove(ContentFilter value)
        {
            this.List.Remove(value);
        }

        public class Enumerator : IEnumerator<ContentFilter>
        {
            private IEnumerator wrapped;

            public Enumerator(ContentFilterCollection collection)
            {
                this.wrapped = ((CollectionBase)collection).GetEnumerator();
            }

            public ContentFilter Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            object IEnumerator.Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            ContentFilter IEnumerator<ContentFilter>.Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            public bool MoveNext()
            {
                return this.wrapped.MoveNext();
            }

            public void Reset()
            {
                this.wrapped.Reset();
            }

            void IDisposable.Dispose()
            {
                this.wrapped.Reset();
            }
        }
     
        public new virtual Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<ContentFilter> IEnumerable<ContentFilter>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
