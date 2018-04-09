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
