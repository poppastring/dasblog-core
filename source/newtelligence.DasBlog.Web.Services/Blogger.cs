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

using CookComputing.XmlRpc;

namespace newtelligence.DasBlog.Web.Services.Blogger
{
    /// <summary>
    /// 
    /// </summary>
    public struct Category
    {
        /// <summary>
        /// 
        /// </summary>
        public string categoryid;
        /// <summary>
        /// 
        /// </summary>
        public string title;
        /// <summary>
        /// 
        /// </summary>
        public string description;
        /// <summary>
        /// 
        /// </summary>
        public string htmlUrl;
        /// <summary>
        /// 
        /// </summary>
        public string rssUrl;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Post
    {
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime dateCreated;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(
             Description="Depending on server may be either string or integer. "
             + "Use Convert.ToInt32(userid) to treat as integer or "
             + "Convert.ToString(userid) to treat as string")]
        public object userid;
        /// <summary>
        /// 
        /// </summary>
        public string postid;
        /// <summary>
        /// 
        /// </summary>
        public string content;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct UserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string url;
        /// <summary>
        /// 
        /// </summary>
        public string email;
        /// <summary>
        /// 
        /// </summary>
        public string nickname;
        /// <summary>
        /// 
        /// </summary>
        public string lastname;
        /// <summary>
        /// 
        /// </summary>
        public string firstname;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BlogInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string blogid;
        /// <summary>
        /// 
        /// </summary>
        public string url;
        /// <summary>
        /// 
        /// </summary>
        public string blogName;
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBlogger
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.deletePost",
             Description="Deletes a post.")]
        [return: XmlRpcReturnValue(Description="Always returns true.")]
        bool blogger_deletePost(
            string appKey,
            string postid,
            string username,
            string password,
            [XmlRpcParameter(
                 Description="Where applicable, this specifies whether the blog "
                 + "should be republished after the post has been deleted.")]
            bool publish);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="content"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.editPost",
             Description="Edits a given post. Optionally, will publish the "
             + "blog after making the edit.")]
        [return: XmlRpcReturnValue(Description="Always returns true.")]
        bool blogger_editPost(
            string appKey,
            string postid,
            string username,
            string password,
            string content,
            bool publish);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getCategories",
             Description="Returns a list of the categories that you can use "
             + "to log against a post.")]
        Category[] blogger_getCategories(
            string blogid,
            string username,
            string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getPost",
             Description="Returns a single post.")]
        Post blogger_getPost(
            string appKey,
            string postid,
            string username,
            string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getRecentPosts",
             Description="Returns a list of the most recent posts in the system.")]
        Post[] blogger_getRecentPosts(
            string appKey,
            string blogid,
            string username,
            string password,
            int numberOfPosts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getTemplate",
             Description="Returns the main or archive index template of "
             + "a given blog.")]
        string blogger_getTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string templateType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUserInfo",
             Description="Authenticates a user and returns basic user info "
             + "(name, email, userid, etc.).")]
        UserInfo blogger_getUserInfo(
            string appKey,
            string username,
            string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUsersBlogs",
             Description="Returns information on all the blogs a given user "
             + "is a member.")]
        BlogInfo[] blogger_getUsersBlogs(
            string appKey,
            string username,
            string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="content"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.newPost",
             Description="Makes a new post to a designated blog. Optionally, "
             + "will publish the blog after making the post.")]
        [return: XmlRpcReturnValue(Description="Id of new post")]
        string blogger_newPost(
            string appKey,
            string blogid,
            string username,
            string password,
            string content,
            bool publish);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="template"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.setTemplate",
             Description="Edits the main or archive index template of a given blog.")]
        bool blogger_setTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string template,
            string templateType);
    }
}
