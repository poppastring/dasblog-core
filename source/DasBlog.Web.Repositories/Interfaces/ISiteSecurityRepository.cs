using DasBlog.Web.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.Repositories.Interfaces
{
    public interface ISiteSecurityRepository
    {
        bool IsValidContributor { get; }

        bool IsInRole(string role);

        void AddUser(string userName, string password, string role, bool ask, string emailAddress);

        void AddUser(User user);

        void UpdateUser(User user);

        void Login(UserToken token, String userName);

        UserToken Login(string userName, string password);

        UserToken Login(string userName, string clientHash, string challenge);

        bool DoSuperChallenge(string challenge, string passwordIN, string userName, string clientHash);

        string Encrypt(string cleanString);

        bool IsCleanStringEncrypted(string cleanString);

        void SetPassword(string userName, string password);

        UserToken GetToken(string userName);

        User GetUser(string userName);

        User GetUserByEmail(string email);

        User GetUserByDisplayName(string displayName);
    }
}
