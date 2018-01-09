using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DasBlog.Web.Core.Configuration
{
    [Serializable]
    public class UserCollection : Collection<User>
    {
        /// <summary>
        /// Initializes a new empty instance of the CrosspostSiteCollection class.
        /// </summary>
        public UserCollection() : base()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the CrosspostSiteCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new CrosspostSiteCollection.
        /// </param>
        public UserCollection(IList<User> items)
            : base()
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this CrosspostSiteCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this CrosspostSiteCollection.
        /// </param>
        public virtual void AddRange(IEnumerable<User> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (User item in items)
            {
                this.Add(item);
            }
        }
    }
}
