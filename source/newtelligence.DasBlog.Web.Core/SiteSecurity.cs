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

namespace newtelligence.DasBlog.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Xml.Serialization;
    using DotNetOpenAuth.OpenId;
    using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
    using DotNetOpenAuth.OpenId.RelyingParty;
    using newtelligence.DasBlog.Runtime;

    public class SiteSecurity
    {
        public static bool IsInRole(string role)
        {
            return HttpContext.Current.User.IsInRole(role);
        }

        public static void SetSecurity(SiteSecurityConfig ssc)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SiteSecurityConfig));
            using (StreamWriter writer = new StreamWriter(SiteConfig.GetSecurityFilePathFromCurrentContext()))
            {
                ser.Serialize(writer, ssc);
            }
        }

        /// <summary>
        /// Shortcut for determining a valid contributor to the blog...currently only users in the "admin" role
        /// or the "contributor" role are supported.
        /// </summary>
        public static bool IsValidContributor()
        {
            return (SiteSecurity.IsInRole("admin") || SiteSecurity.IsInRole("contributor"));
        }

        public static SiteSecurityConfig GetSecurity()
        {

            string fullPath = SiteConfig.GetSecurityFilePathFromCurrentContext();
            //if (HttpContext.Current != null)
            //{
            //    fullPath = HttpContext.Current.Server.MapPath("~/SiteConfig/siteSecurity.config");
            //}
            //else //We are being call on a background thread...
            //{	
            //    fullPath = Path.Combine(HttpRuntime.AppDomainAppPath,"SiteConfig/siteSecurity.config");
            //}

            // luke@jurasource.co.uk 18-MAY-04
            // When the current page is below the root directory, try the directory above
            // for the location of the security config file.
            //
            // Typically the siteSecurity.config file will not exist in the cases where
            // the page checking the current user is below the root web directory.  For
            // example, /ftb/ftb.insertcode.aspx, or /ftb/ftb.imagegallery.aspx
            //
            // Note that I have only seen this problem when NT Authentication is being
            // used at the site level.

            /* Do we really need this?

			if ( !File.Exists(fullPath) )
			{
				fullPath = HttpContext.Current.Server.MapPath("~/SiteConfig/siteSecurity.config");
			}
             */

            if (File.Exists(fullPath))
            {
                return GetSecurity(fullPath);
            }

            return new SiteSecurityConfig();
        }

        // make it easier to get the sitesecurity from a non-web app.
        public static SiteSecurityConfig GetSecurity(string path)
        {

            // argument validation
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            // setup the serializer
            XmlSerializer ser = new XmlSerializer(typeof(SiteSecurityConfig));

            // load the security config
            using (StreamReader reader = new StreamReader(path))
            {
                return (SiteSecurityConfig)ser.Deserialize(reader);
            }
        }

        public static void AddUser(string userName, string password, string role, bool ask, string emailAddress)
        {
            if (string.IsNullOrEmpty(userName)) { throw new ArgumentNullException("userName"); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException("password"); }
            
            User user = new User();
            user.Name = userName;
            user.Password = password;
            user.Role = role ?? "contributor";
            user.Ask = ask;
            user.EmailAddress = emailAddress;

            AddUser(user);
        }

        /// <summary>
        /// Adds a new user to the current collection of users.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public static void AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            SiteSecurityConfig ssc = GetSecurity();
            ssc.Users.Add(user);

            SetSecurity(ssc);
        }

        /// <summary>
        /// Finds the user in the current user collection and replaces him with the supplied user.
        /// The mapping is done by the username.
        /// </summary>
        /// <param name="user">The user to update.</param>
        public static void UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            SiteSecurityConfig ssc = GetSecurity();

            int index = ssc.Users.FindIndex(delegate(User x)
            {
                return String.Compare(x.Name, user.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
            });

            if (index >= 0)
            {
                ssc.Users[index] = user;
                SetSecurity(ssc);
            }
        }


        private static void Login(UserToken token, String userName)
        {
            if (token == null)
            {
                SiteSecurity.LogFailure(userName);
            }
            else
            {
                SiteSecurity.LogSuccess(token.Name);
                GenericIdentity identity = new GenericIdentity(token.Name, "Custom");
                GenericPrincipal principal = new GenericPrincipal(identity, new string[] { token.Role });
                HttpContext.Current.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }
        }

        public static UserToken Login(IAuthenticationResponse resp)
        {
            if (resp == null)
            {
                return null;
            }

            UserToken token = null;
            if (resp.Status == AuthenticationStatus.Authenticated)
            {
                ClaimsResponse ProfileFields = resp.GetExtension<ClaimsResponse>();

                User user = GetUserByOpenIDIdentifier(resp.ClaimedIdentifier);

                if (user != null && user.Active)
                {
                    token = user.ToToken();
                }
            }

            Login(token, resp.ClaimedIdentifier);

            return token;
        }

        /// <summary>
        /// This function takes a password and the userName to
        /// compare the password with the password asigned to the userName.
        /// Both passwords, only one or none will exist as md5 hashed.  
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>user as UserToken.</returns>
        public static UserToken Login(string userName, string password)
        {
            UserToken token = null;
            SiteSecurityConfig ssc = GetSecurity();
            /*
            foreach (User user in ssc.Users)
            {
                if (user.Name.ToUpper() == userName.ToUpper() && user.Active)
                {
                    if ((IsCleanStringEncrypted(user.Password) && IsCleanStringEncrypted(password)) ||
                        (!IsCleanStringEncrypted(user.Password) && !IsCleanStringEncrypted(password)))
                    {
                        if (user.Password == password)
                        {
                            token = user.ToToken();
                            break;
                        }
                        else if (user.Password == SiteSecurity.Encrypt(password))
                        {
                            token = user.ToToken();
                            break;
                        }
                    }
                    else if ((IsCleanStringEncrypted(user.Password) && !IsCleanStringEncrypted(password)))
                    {
                        if (user.Password == Encrypt(password))
                        {
                            token = user.ToToken();
                            break;
                        }
                    }
                    else
                    {
                        if (Encrypt(user.Password) == password)
                        {
                            token = user.ToToken();
                            break;
                        }
                    }
                }
            }
             * */
            User user = GetUser(userName);

            if (user != null && user.Active)
            {
                //Make sure password is encrypted
                if (!IsCleanStringEncrypted(password))
                {
                    password = SiteSecurity.Encrypt(password);
                }
                //if the stored password is encrypted, test equality, or test equality with the encrypted version of it
                if ((IsCleanStringEncrypted(user.Password) && user.Password == password) || (SiteSecurity.Encrypt(user.Password) == password))
                {
                    token = user.ToToken();
                }
            }

            Login(token, userName);

            return token;
        }

        private static void LogFailure(string userName)
        {
            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            if (HttpContext.Current == null)
            {
                loggingService.AddEvent(new EventDataItem(EventCodes.SecurityFailure, userName, "non-web"));
            }
            else
            {
                loggingService.AddEvent(new EventDataItem(EventCodes.SecurityFailure, userName, HttpContext.Current.Request.UserHostAddress));
            }
        }

        private static void LogSuccess(string userName)
        {
            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            if (HttpContext.Current == null)
            {
                loggingService.AddEvent(new EventDataItem(EventCodes.SecuritySuccess, userName, "non-web"));
            }
            else
            {
                loggingService.AddEvent(new EventDataItem(EventCodes.SecuritySuccess, userName, HttpContext.Current.Request.UserHostAddress));
            }
        }

        /// <summary>
        /// This function takes a password, the  challenge and the userName to
        /// make an super challenge like on the client side. 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="clientHash"></param>
        /// <param name="challenge"></param>
        /// <returns>user as UserToken.</returns>
        public static UserToken Login(string userName, string clientHash, string challenge)
        {
            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());

            UserToken token = null;
            /*
			SiteSecurityConfig ssc = GetSecurity();
			foreach (User user in ssc.Users)
			{
                if (user.Active && user.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
				{
					if (DoSuperChallenge(challenge, user.Password, userName, clientHash))
					{
						token = user.ToToken();
						break;
					}
				}
			}
             */
            User user = GetUser(userName);
            if (user != null && user.Active && DoSuperChallenge(challenge, user.Password, userName, clientHash))
            {
                token = user.ToToken();
            }

            if (token == null)
            {
                SiteSecurity.LogFailure(userName);
            }
            else
            {
                SiteSecurity.LogSuccess(token.Name);
                GenericIdentity identity = new GenericIdentity(token.Name, "Custom");
                GenericPrincipal principal = new GenericPrincipal(identity, new string[] { token.Role });
                HttpContext.Current.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }

            return token;
        }

        /// <summary>
        /// This function builds from the challenge, the (md5 hashed) password and the userName
        /// an md5 hashed string and compare them with the cryptPassword from the client. 
        /// </summary>
        /// <param name="challenge"></param>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <param name="clientHash"></param>
        /// <returns>true if coparing was good otherwise false</returns>
        public static bool DoSuperChallenge(string challenge, string passwordIN, string userName, string clientHash)
        {
            string password = Encrypt(passwordIN);
            string serverHash = Encrypt(challenge + password + userName);

            return (clientHash == serverHash);

        }

        /// <summary>
        /// This function compute a given string with the md5 hash algorithm. 
        /// </summary>
        /// <param name="cleanString"></param>
        /// <returns>hash from cleanString as string.</returns>
        public static string Encrypt(string cleanString)
        {
            if (!IsCleanStringEncrypted(cleanString))
            {
                Byte[] clearBytes = Encoding.Unicode.GetBytes(cleanString);

                MD5 md5 = MD5CryptoServiceProvider.Create();
                Byte[] hashedBytes = md5.ComputeHash(clearBytes);

                return BitConverter.ToString(hashedBytes);
            }
            else
                return cleanString;
        }

        /// <summary>
        /// This function checks an given string if it is always encrypted 
        /// </summary>
        /// <param name="cleanString">clean string to check</param>
        /// <returns>true or false depending on string type.</returns>
        public static bool IsCleanStringEncrypted(string cleanString)
        {
            return (cleanString.Length == 47 && cleanString.Replace("-", String.Empty).Length == 32);
        }

        /// <summary>
        /// This function takes a password and then
        /// writes the siteSecurity.config file.
        /// </summary>
        /// <param name="userName">The username of the logged in user</param>
        /// <param name="password">The plain text password of the logged in user</param>
        public static void SetPassword(string userName, string password)
        {
            SiteSecurityConfig ssc = GetSecurity();
            User user = GetUser(userName);
            user.Password = password;

            // write out the changes
            SetSecurity(ssc);
        }

        public static UserToken GetToken(string userName)
        {
            User user = GetUser(userName);
            return user == null ? null : user.ToToken();
        }

        /// <summary>
        /// This function looks up a given username and returns the associated
        /// User object.
        /// </summary>
        /// <param name="userName">The username to look up.</param>
        /// <returns>The User object corresponding to the provided username.</returns>
        public static User GetUser(string userName)
        {
            if (false == String.IsNullOrEmpty(userName))
            {
                SiteSecurityConfig ssc = GetSecurity();

                return ssc.Users.Find(delegate(User x)
                {
                    return String.Compare(x.Name, userName, StringComparison.InvariantCultureIgnoreCase) == 0;
                });
            }
            return null;
        }

        /// <summary>
        /// This function looks up a given email and returns the associated
        /// User object.
        /// </summary>
        /// <param name="userName">The email to look up.</param>
        /// <returns>The User object corresponding to the provided email.</returns>
        public static User GetUserByEmail(string email)
        {
            if (false == String.IsNullOrEmpty(email))
            {
                SiteSecurityConfig ssc = GetSecurity();

                return ssc.Users.Find(delegate(User x)
                {
                    return String.Compare(x.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase) == 0;
                });
            }
            return null;
        }

        /// <summary>
        /// This function looks up a given displayname and returns the associated
        /// User object.
        /// </summary>
        /// <param name="userName">The displayname to look up.</param>
        /// <returns>The User object corresponding to the provided displayname.</returns>
        public static User GetUserByDisplayName(string displayName)
        {

            if (false == String.IsNullOrEmpty(displayName))
            {
                SiteSecurityConfig ssc = GetSecurity();
                return ssc.Users.Find(delegate(User x)
                {
                    return String.Compare(x.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0;
                });
            }

            return null;
        }

        /// <summary>
        /// This function looks up a given OpenId identifier and returns the associated
        /// User object.
        /// </summary>
        /// <param name="userName">The OpenId identifier to look up.</param>
        /// <returns>The User object corresponding to the provided OpenId identifier.</returns>
        public static User GetUserByOpenIDIdentifier(Identifier identifier)
        {
            if (identifier != null)
            {
                SiteSecurityConfig ssc = GetSecurity();
                return ssc.Users.Find(delegate(User x)
                {
                    return Identifier.Parse(x.OpenIDUrl) == identifier;
                });
            }

            return null;
        }
    }

    public class SiteSecurityConfig
    {
        public SiteSecurityConfig() { }

        UserCollection users = new UserCollection();

        public UserCollection Users { get { return users; } }
    }

    public sealed class UserToken
    {
        public UserToken(string name, string role)
        {
            this.Name = name;
            this.Role = role;
        }


        string name;
        string role;

        public string Name { get { return name; } private set { name = value; } }
        public string Role { get { return role; } private set { role = value; } }
    }

    public class User
    {
        string name;
        string password;
        string role;
        bool ask;
        string emailAddress;
        string displayName;
        string openIDUrl = "";

        bool notifyOnNewPost = false;
        bool notifyOnAllComment = false;
        bool notifyOnOwnComment = false;
        bool active = true;

        public string Name { get { return name; } set { name = value; } }
        [XmlIgnore]
        public string Password { get { return password; } set { password = SiteSecurity.Encrypt(value); } }
        public string Role { get { return role; } set { role = value; } }
        public bool Ask { get { return ask; } set { ask = value; } }
        public string EmailAddress { get { return emailAddress; } set { emailAddress = value; } }
        public string DisplayName { get { return displayName; } set { displayName = value; } }
        public string OpenIDUrl { get { return openIDUrl; } set { openIDUrl = value; } }

        public bool NotifyOnNewPost { get { return notifyOnNewPost; } set { notifyOnNewPost = value; } }
        public bool NotifyOnAllComment { get { return notifyOnAllComment; } set { notifyOnAllComment = value; } }
        public bool NotifyOnOwnComment { get { return notifyOnOwnComment; } set { notifyOnOwnComment = value; } }
        public bool Active { get { return active; } set { active = value; } }

        /// <summary>
        /// Encrypted password for serialization.
        /// </summary>
        [XmlElement("Password")]
        public string XmlPassword
        {
            get
            {
                return Password;
            }
            set
            {
                password = value;
            }
        }

        public UserToken ToToken()
        {
            UserToken token = new UserToken(name, role);
            //token.Name = name;
            //token.Role = role;
            return token;
        }
    }

    #region UserCollection
    /// <summary>
    /// A collection of elements of type User
    /// </summary>
    public class UserCollection : System.Collections.Generic.List<User>
    {
        /// <summary>
        /// Initializes a new empty instance of the UserCollection class.
        /// </summary>
        public UserCollection()
            : base()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the UserCollection class, containing elements
        /// copied from the supplied array.
        /// </summary>
        /// <param name="items">
        /// The collection whose elements are to be added to the new UserCollection.
        /// </param>
        public UserCollection(IEnumerable<User> items)
        {
            this.AddRange(items);
        }
    }
    #endregion
}
