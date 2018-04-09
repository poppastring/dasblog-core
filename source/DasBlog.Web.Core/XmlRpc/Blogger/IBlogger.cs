using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.Blogger
{
    public interface IBlogger
    {
        [XmlRpcMethod("blogger.deletePost",Description = "Deletes a post.")]
        [return: XmlRpcReturnValue(Description = "Always returns true.")]
        bool blogger_deletePost(string appKey, string postid, string username, string password,
            [XmlRpcParameter(Description="Where applicable, this specifies whether the blog should be republished after the post has been deleted.")]
            bool publish);

        [XmlRpcMethod("blogger.editPost",
             Description = "Edits a given post. Optionally, will publish the blog after making the edit.")]
        [return: XmlRpcReturnValue(Description = "Always returns true.")]
        bool blogger_editPost(
            string appKey,
            string postid,
            string username,
            string password,
            string content,
            bool publish);

        [XmlRpcMethod("blogger.getCategories", Description = "Returns a list of the categories that you can use to log against a post.")]
        Category[] blogger_getCategories(
            string blogid,
            string username,
            string password);

        [XmlRpcMethod("blogger.getPost", Description = "Returns a single post.")]
        Post blogger_getPost(
            string appKey,
            string postid,
            string username,
            string password);

        /// <returns></returns>
        [XmlRpcMethod("blogger.getRecentPosts", Description = "Returns a list of the most recent posts in the system.")]
        Post[] blogger_getRecentPosts(
            string appKey,
            string blogid,
            string username,
            string password,
            int numberOfPosts);

        [XmlRpcMethod("blogger.getTemplate",
             Description = "Returns the main or archive index template of "
             + "a given blog.")]
        string blogger_getTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string templateType);

        [XmlRpcMethod("blogger.getUserInfo",
             Description = "Authenticates a user and returns basic user info "
             + "(name, email, userid, etc.).")]
        UserInfo blogger_getUserInfo(
            string appKey,
            string username,
            string password);

        [XmlRpcMethod("blogger.getUsersBlogs", Description = "Returns information on all the blogs a given user is a member.")]
        BlogInfo[] blogger_getUsersBlogs(
            string appKey,
            string username,
            string password);

        [XmlRpcMethod("blogger.newPost", Description = "Makes a new post to a designated blog. Optionally, "
             + "will publish the blog after making the post.")]
        [return: XmlRpcReturnValue(Description = "Id of new post")]
        string blogger_newPost(
            string appKey,
            string blogid,
            string username,
            string password,
            string content,
            bool publish);

        [XmlRpcMethod("blogger.setTemplate", Description = "Edits the main or archive index template of a given blog.")]
        bool blogger_setTemplate(
            string appKey,
            string blogid,
            string username,
            string password,
            string template,
            string templateType);
    }
}
