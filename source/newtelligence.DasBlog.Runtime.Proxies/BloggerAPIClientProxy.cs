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

using System;
using CookComputing.XmlRpc;

namespace newtelligence.DasBlog.Runtime.Proxies
{

    /// <summary>
    /// 
    /// </summary>
    public struct mtCategory
    {
        /// <summary>
        /// 
        /// </summary>
        public string categoryId;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string categoryName;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool isPrimary;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct mtPostTitle
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="This is in the timezone of the weblog blogid.")]
        public DateTime created;
        /// <summary>
        /// 
        /// </summary>
        public string postid;
        /// <summary>
        /// 
        /// </summary>
        public string userid;
        /// <summary>
        /// 
        /// </summary>
        public string title;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct mtTrackbackPing
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="The title of the entry sent in the ping.")]
        public string pingTitle;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="The URL of the entry.")]
        public string pingURL;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="The IP address of the host that sent the ping.")]
        public string pingIP;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct mtTextFilter
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="unique string identifying a text formatting plugin")]
        public string key;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(Description="readable description to be displayed to a user")]
        public string value;
    }


    /// <summary>
    /// 
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct mwEnclosure
    {
        /// <summary>
        /// 
        /// </summary>
        public int length;
        /// <summary>
        /// 
        /// </summary>
        public string type;
        /// <summary>
        /// 
        /// </summary>
        public string url;
    }

    /// <summary>
    /// 
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct mwSource
    {
        /// <summary>
        /// 
        /// </summary>
        public string name;
        /// <summary>
        /// 
        /// </summary>
        public string url;
    }

    /// <summary>
    /// 
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct mwPost
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description="Required when posting.")]
        public DateTime dateCreated;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description="Required when posting.")]
        public string description;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description="Required when posting.")]
        public string title;

        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
        public string[] categories;

        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
        public mwEnclosure enclosure;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
        public string link;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
        public string permalink;
        [XmlRpcMember(
             Description="Not required when posting. Depending on server may "
             + "be either string or integer. "
             + "Use Convert.ToInt32(postid) to treat as integer or "
             + "Convert.ToString(postid) to treat as string")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string postid;

        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public mwSource source;
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public string userid;

        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
		public string mt_allow_comments;
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public int mt_allow_pings;
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public string mt_convert_breaks;
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public string mt_text_more;
        [XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]    
        public string mt_excerpt;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct mwCategoryInfo
    {
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
        /// <summary>
        /// 
        /// </summary>
        public string title;
        /// <summary>
        /// 
        /// </summary>
        public string categoryid;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct mwCategory
    {
        /// <summary>
        /// 
        /// </summary>
        public string categoryId;
        /// <summary>
        /// 
        /// </summary>
        public string categoryName;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct bgCategory
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
    public struct bgPost
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
    public struct bgUserInfo
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
    public struct bgBlogInfo
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
	/// Summary description for BloggerAPIClientProxy.
	/// </summary>
	public class BloggerAPIClientProxy : XmlRpcClientProtocol
	{
		public BloggerAPIClientProxy()
		{
			
		}

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
        public bool blogger_deletePost(
            string appKey,
            string postid,
            string username,
            string password,
            [XmlRpcParameter(
                 Description="Where applicable, this specifies whether the blog "
                 + "should be republished after the post has been deleted.")]
            bool publish)
        {
            return (bool)Invoke("blogger_deletePost", new object[]{appKey,postid,username,password,publish});
        }

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
        public bool blogger_editPost(
            string appKey,
            string postid,
            string username,
            string password,
            string content,
            bool publish)
        {
            return (bool)Invoke("blogger_editPost", new object[]{appKey,postid,username,password,content,publish});
        }

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
        public bgCategory[] blogger_getCategories(
            string blogid,
            string username,
            string password)
        {
            return (bgCategory[])Invoke("blogger_getCategories", new object[]{blogid,username,password});
        }

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
        public bgPost blogger_getPost(
            string appKey,
            string postid,
            string username,
            string password)
        {
            return (bgPost)Invoke("blogger_getPost", new object[]{appKey,postid,username,password});
        }

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
        public bgPost[] blogger_getRecentPosts(
            string appKey,
            string blogid,
            string username,
            string password,
            int numberOfPosts)
        {
            return (bgPost[])Invoke("blogger_getRecentPosts", new object[]{appKey,blogid,username,password,numberOfPosts});
        }

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
        public string blogger_getTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string templateType)
        {
            return (string)Invoke("blogger_getTemplate", new object[]{appKey,blogid,username,password,templateType});
        }

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
        public bgUserInfo blogger_getUserInfo(
            string appKey,
            string username,
            string password)
        {
            return (bgUserInfo)Invoke("blogger_getUserInfo", new object[]{appKey,username,password});
        }

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
        public bgBlogInfo[] blogger_getUsersBlogs(
            string appKey,
            string username,
            string password)
        {
            return (bgBlogInfo[])Invoke("blogger_getUsersBlogs", new object[]{appKey,username,password});
        }

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
        public string blogger_newPost(
            string appKey,
            string blogid,
            string username,
            string password,
            string content,
            bool publish)
        {
            return (string)Invoke("blogger_newPost", new object[]{appKey,blogid,username,password,content,publish});
        }

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
        public bool blogger_setTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string template,
            string templateType)
        {
            return (bool)Invoke("blogger_setTemplate", new object[]{appKey,blogid,username,password,template,templateType});
        }

        /// <summary>
        /// Updates an existing post to a designated blog using the metaWeblog API. Returns true if completed.
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns>true if successful, false otherwise</returns>
        [XmlRpcMethod("metaWeblog.editPost",
             Description="Updates an existing post to a designated blog "
             + "using the metaWeblog API. Returns true if completed.")]
        public bool metaweblog_editPost(
            string postid,
            string username,
            string password,
            mwPost post,
            bool publish)
        {
            return (bool)Invoke("metaweblog_editPost", new object[]{postid,username,password,post,publish});
        }

        /// <summary>
        ///  Retrieves a list of valid categories for a post 
        /// using the metaWeblog API. Returns the metaWeblog categories
        /// struct collection.
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getCategories",
             Description="Retrieves a list of valid categories for a post "
             + "using the metaWeblog API. Returns the metaWeblog categories "
             + "struct collection.")]
        public mwCategoryInfo[] metaweblog_getCategories(
            string blogid,
            string username,
            string password)
        {
            return (mwCategoryInfo[])Invoke("metaweblog_getCategories", new object[]{blogid,username,password});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getPost",
             Description="Retrieves an existing post using the metaWeblog "
             + "API. Returns the metaWeblog struct.")]
        public mwPost metaweblog_getPost(
            string postid,
            string username,
            string password)
        {
            return (mwPost)Invoke("metaweblog_getPost", new object[]{postid,username,password});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getRecentPosts",
             Description="Retrieves a list of the most recent existing post "
             + "using the metaWeblog API. Returns the metaWeblog struct collection.")]
        public mwPost[] metaweblog_getRecentPosts(
            string blogid,
            string username,
            string password,
            int numberOfPosts)
        {
            return (mwPost[])Invoke("metaweblog_getRecentPosts", new object[]{blogid,username,password,numberOfPosts});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.newPost",
             Description="Makes a new post to a designated blog using the "
             + "metaWeblog API. Returns postid as a string.")]
        public string metaweblog_newPost(
            string blogid,
            string username,
            string password,
            mwPost post,
            bool publish)
        {
            return (string)Invoke("metaweblog_newPost", new object[]{blogid,username,password,post,publish});
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.getCategoryList",
             Description="Returns a list of all categories defined in the weblog.")]
        [return: XmlRpcReturnValue(
                     Description="The isPrimary member of each mtCategory structs is not used.")]
        public mtCategory[] mt_getCategoryList(
            string blogid,
            string username,
            string password)
        {
            return (mtCategory[])Invoke("mt_getCategoryList", new object[]{blogid,username,password});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.getPostCategories",
             Description="Returns a list of all categories to which the post is "
             + "assigned.")]
        public mtCategory[] mt_getPostCategories(
            string postid,
            string username,
            string password)
        {
            return (mtCategory[])Invoke("mt_getPostCategories", new object[]{postid,username,password});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.getRecentPostTitles",
             Description="Returns a bandwidth-friendly list of the most recent "
             + "posts in the system.")]
        public mtPostTitle[] mt_getRecentPostTitles(
            string blogid,
            string username,
            string password,
            int numberOfPosts)
        {
            return (mtPostTitle[])Invoke("mt_getRecentPostTitles", new object[]{blogid,username,password});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.getTrackbackPings",
             Description="Retrieve the list of TrackBack pings posted to a "
             + "particular entry. This could be used to programmatically "
             + "retrieve the list of pings for a particular entry, then "
             + "iterate through each of those pings doing the same, until "
             + "one has built up a graph of the web of entries referencing "
             + "one another on a particular topic.")]
        public mtTrackbackPing[] mt_getTrackbackPings(
            string postid)
        {
            return (mtTrackbackPing[])Invoke("mt_getTrackbackPings", new object[]{postid});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.publishPost",
             Description="Publish (rebuild) all of the static files related "
             + "to an entry from your weblog. Equivalent to saving an entry "
             + "in the system (but without the ping).")]
        [return: XmlRpcReturnValue(Description="Always returns true.")]
        public bool mt_publishPost(
            string postid,
            string username,
            string password)
        {
            return (bool)Invoke("mt_publishPost", new object[]{postid,username,password});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        [XmlRpcMethod("mt.setPostCategories",
             Description="Sets the categories for a post.")]
        [return: XmlRpcReturnValue(Description="Always returns true.")]
        public bool mt_setPostCategories(
            string postid,
            string username,
            string password,
            [XmlRpcParameter(
                 Description="categoryName not required in mtCategory struct.")]
            mtCategory[] categories)
        {
            return (bool)Invoke("mt_setPostCategories", new object[]{postid,username,password,categories});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [XmlRpcMethod("mt.supportedMethods",
             Description="The method names supported by the server.")]
        [return: XmlRpcReturnValue(
                     Description="The method names supported by the server.")]
        public string[] mt_supportedMethods()
        {
            return (string[])Invoke("mt_supportedMethods", new object[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        
        [XmlRpcMethod("mt.supportedTextFilters",
             Description="The text filters names supported by the server.")]
        [return: XmlRpcReturnValue(
                     Description="The text filters names supported by the server.")]
        public mtTextFilter[] mt_supportedTextFilters()
            
        {
            return (mtTextFilter[])Invoke("mt_supportedTextFilters", new object[0]);
        }
    }

}
