using System.Collections.Generic;

namespace DasBlog.Web.UI.Core.Security
{
    public class UserCollection : List<User>
    {
        public UserCollection() : base() { }

        public UserCollection(IEnumerable<User> items) => this.AddRange(items);
    }
}
