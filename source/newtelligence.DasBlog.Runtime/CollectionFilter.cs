using System;
using System.Collections;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
    /// <summary>
    /// Containes methods to filter collections.
    /// 
    /// </summary>
    public class CollectionFilter<U, T> where U : IList<T>, IList, new()
    {

        /// <summary>
        /// Create a filtered collection from the original collection where the include delegate
        /// indicates which items to include in the filtered collection.
        /// </summary>
        /// <param name="source">The collection to search.</param>
        /// <param name="include">A delegate that returns true for each item that should be left in the collection.</param>
        /// <returns>A collection containing the items matching the criteria.</returns>
        public static U FindAll(U source, Predicate<T> include)
        {
            return FindAll(source, include, Int32.MaxValue);
        }

        /// <summary>
        /// Create a filtered collection from the original collection where the include delegate
        /// indicates which items to include in the filtered collection.
        /// </summary>
        /// <param name="source">The collection to search.</param>
        /// <param name="include">A delegate that returns true for each item that should be left in the collection.</param>
        /// <param name="maxResults">Maximum number of results to return.</param>
        /// <returns>A collection containing the items matching the criteria.</returns>
        public static U FindAll(U source, Predicate<T> include, int maxResults)
        {

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            U filteredCollection = new U();
            bool includeThisItem;

            foreach (T entry in source)
            {
                if (include != null)
                {
                    includeThisItem = true;

                    // Since include could be a chain of multiple delegates we 
                    // have to loop through them all.  If any delegate returns
                    // false the item is not included.
                    foreach (Predicate<T> includeDelegate in include.GetInvocationList())
                    {
                        includeThisItem = includeThisItem && includeDelegate(entry);
                        if (!includeThisItem)
                        {
                            break;
                        }
                    }
                    if (includeThisItem)
                    {
                        filteredCollection.Add(entry);
                    }

                }
                else
                {
                    //filteredCollection = new U();
                    //foreach (T item in source)
                    //{
                    filteredCollection.Add(entry);
                    //}
                }
            

                if (((ICollection<T>)filteredCollection).Count >= maxResults)
                {
                    break;
                }
            }

            return filteredCollection;
        }
    }


}
