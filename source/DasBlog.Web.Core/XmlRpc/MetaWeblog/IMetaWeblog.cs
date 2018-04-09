using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.MetaWeblog
{
    public interface IMetaWeblog
    {
        /// <summary>
        /// Updates an existing post to a designated blog using the metaWeblog API. Returns true if completed.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        [XmlRpcMethod("metaWeblog.editPost", Description = "Updates an existing post to a designated blog "
             + "using the metaWeblog API. Returns true if completed.")]
        bool metaweblog_editPost(
            string postid,
            string username,
            string password,
            Post post,
            bool publish);

        /// <summary>
        ///  Retrieves a list of valid categories for a post 
        /// using the metaWeblog API. Returns the metaWeblog categories
        /// struct collection.
        /// </summary>
        [XmlRpcMethod("metaWeblog.getCategories",
             Description = "Retrieves a list of valid categories for a post "
             + "using the metaWeblog API. Returns the metaWeblog categories "
             + "struct collection.")]
        CategoryInfo[] metaweblog_getCategories(
            string blogid,
            string username,
            string password);


        [XmlRpcMethod("metaWeblog.getPost",
             Description = "Retrieves an existing post using the metaWeblog "
             + "API. Returns the metaWeblog struct.")]
        Post metaweblog_getPost(
            string postid,
            string username,
            string password);

        [XmlRpcMethod("metaWeblog.getRecentPosts",
             Description = "Retrieves a list of the most recent existing post "
             + "using the metaWeblog API. Returns the metaWeblog struct collection.")]
        Post[] metaweblog_getRecentPosts(
            string blogid,
            string username,
            string password,
            int numberOfPosts);

        [XmlRpcMethod("metaWeblog.newPost",
             Description = "Makes a new post to a designated blog using the "
             + "metaWeblog API. Returns postid as a string.")]
        string metaweblog_newPost(
            string blogid,
            string username,
            string password,
            Post post,
            bool publish);

        /// <summary>
        /// newMediaObject implementation : Xas
        /// </summary>
        [XmlRpcMethod("metaWeblog.newMediaObject",
             Description = "Upload a new file to the binary content. Returns url as a string")]
        UrlInfo metaweblog_newMediaObject(object blogid, string username, string password, MediaType enc);
    }
}
