using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.Security
{
    public sealed class UserToken
    {
        public UserToken(string name, string role)
        {
            this.Name = name;
            this.Role = role;
        }

        public string Name { get; private set; }
        public string Role { get; private set; }
    }
}
