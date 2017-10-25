using System;
using System.Collections;
using System.Diagnostics;
using System.Web.Caching;
using System.Web.Hosting;

namespace newtelligence.DasBlog.Web.Core
{

    /// <summary>
    /// Wrapper for the HostingEnvironment.Cache.
    /// </summary>
    public sealed class HttpCache : DataCache
    {

        /// <summary>
        /// Creates a new instance of the <see cref="HttpCache" /> class.
        /// </summary>
        internal HttpCache()
        {

            cache = HostingEnvironment.Cache;

            Debug.Assert(cache != null);
            if (cache == null)
            {
                throw new NotSupportedException("Current hosting environment does not support caching.");
            }
        }

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        public override void Clear()
        {

            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            if (CacheEnum != null)
            {
                while (CacheEnum.MoveNext())
                {
                    string key = CacheEnum.Key as String;
                    if (key != null)
                    {
                        cache.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public override void Insert(string key, object value, CacheItemPriority priority)
        {

            cache.Insert(key, value, null, NoAbsoluteExpiration, NoSlidingExpiration, priority, null);
        }

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public override void Insert(string key, object value, CacheDependency dependencies)
        {

            cache.Insert(key, value, dependencies);
        }

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public override void Insert(string key, object value, DateTime absoluteExpiration)
        {
            cache.Insert(key, value, null, absoluteExpiration, NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public override void Insert(string key, object value, TimeSpan slidingExpiration)
        {
            cache.Insert(key, value, null, NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Removes an object from the cache.
        /// </summary>
        public override object Remove(string key)
        {

            return cache.Remove(key);
        }

        /// <summary>
        /// Gets the object specified by the <paramref name="key" /> from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object this[string key]
        {
            get
            {
                return cache[key];
            }
        }


        private System.Web.Caching.Cache cache;

    }

    public abstract class DataCache
    {

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public abstract void Insert(string key, object value, CacheItemPriority priority);

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public abstract void Insert(string key, object value, CacheDependency dependency);

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public abstract void Insert(string key, object value, DateTime absoluteExpiration);

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public abstract void Insert(string key, object value, TimeSpan slidingExpiration);

        /// <summary>
        ///  Inserts an object in the Cache, overrides an existing object with the same key.
        /// </summary>
        public abstract object Remove(string key);


        /// <summary>
        /// Gets the object specified by the <paramref name="key" /> from the cache.
        /// </summary>
        public abstract object this[string key]
        {
            get;
        }

        /// <summary>
        /// Indicates the item never expires.
        /// </summary>
        protected static DateTime NoAbsoluteExpiration
        {
            get
            {
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        ///  Disables sliding expiration.
        /// </summary>
        protected static TimeSpan NoSlidingExpiration
        {
            get
            {
                return TimeSpan.Zero;
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public static class CacheFactory
    {
        private static volatile DataCache cache;
        private static object syncRoot = new object();

        /// <summary>
        /// Creates an instance of the cache used by the application.
        /// </summary>
        public static DataCache GetCache()
        {

            if (cache == null)
            {
                lock (syncRoot)
                {
                    if (cache == null)
                    {
                        cache = new HttpCache();
                    }
                }
            }

            return cache;
        }
    }
}
