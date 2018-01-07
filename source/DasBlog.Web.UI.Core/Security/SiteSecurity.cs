using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using DasBlog.Web.UI.Core.Configuration;

namespace DasBlog.Web.UI.Core.Security
{
    public class SiteSecurity : ISiteSecuriy
    {
        private IPrincipal _principal;

        public SiteSecurity(IPrincipal principal)
        {
            _principal = principal;
        }

        public bool IsValidContributor { get { return (_principal.IsInRole("") || _principal.IsInRole("")); } }

        public bool IsInRole(string role)
        {
            return _principal.IsInRole(role);
        }

        public void AddUser(string userName, string password, string role, bool ask, string emailAddress)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool DoSuperChallenge(string challenge, string passwordIN, string userName, string clientHash)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string cleanString)
        {
            throw new NotImplementedException();
        }

        public SiteSecurityConfig GetSecurity()
        {
            throw new NotImplementedException();
        }

        public SiteSecurityConfig GetSecurity(string path)
        {
            throw new NotImplementedException();
        }

        public UserToken GetToken(string userName)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string userName)
        {
            throw new NotImplementedException();
        }

        public User GetUserByDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public bool IsCleanStringEncrypted(string cleanString)
        {
            throw new NotImplementedException();
        }

        public void Login(UserToken token, string userName)
        {
            throw new NotImplementedException();
        }

        public UserToken Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public UserToken Login(string userName, string clientHash, string challenge)
        {
            throw new NotImplementedException();
        }

        public void SetPassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void SetSecurity(SiteSecurityConfig ssc)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
