using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.MoveableType
{
    public interface IMovableType
    {
        [XmlRpcMethod("mt.getCategoryList",
            Description = "Returns a list of all categories defined in the weblog.")]
        [return: XmlRpcReturnValue(Description = "The isPrimary member of each Category structs is not used.")]
        Category[] mt_getCategoryList(string blogid, string username, string password);


        [XmlRpcMethod("mt.getPostCategories",
            Description = "Returns a list of all categories to which the post is assigned.")]
        Category[] mt_getPostCategories(string postid, string username, string password);


        [XmlRpcMethod("mt.getRecentPostTitles",
            Description = "Returns a bandwidth-friendly list of the most recent posts in the system.")]
        PostTitle[] mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts);

        [XmlRpcMethod("mt.getTrackbackPings",
            Description = "Retrieve the list of TrackBack pings posted to a "
             + "particular entry. This could be used to programmatically "
             + "retrieve the list of pings for a particular entry, then "
             + "iterate through each of those pings doing the same, until "
             + "one has built up a graph of the web of entries referencing "
             + "one another on a particular topic.")]
        TrackbackPing[] mt_getTrackbackPings(string postid);

        [XmlRpcMethod("mt.publishPost",
             Description = "Publish (rebuild) all of the static files related "
             + "to an entry from your weblog. Equivalent to saving an entry "
             + "in the system (but without the ping).")]
        [return: XmlRpcReturnValue(Description = "Always returns true.")]
        bool mt_publishPost(string postid, string username, string password);

        [XmlRpcMethod("mt.setPostCategories",
             Description = "Sets the categories for a post.")]
        [return: XmlRpcReturnValue(Description = "Always returns true.")]
        bool mt_setPostCategories(string postid, string username, string password,
            [XmlRpcParameter(Description="categoryName not required in Category struct.")]
            Category[] categories);

        [XmlRpcMethod("mt.supportedMethods", Description = "The method names supported by the server.")]
        [return: XmlRpcReturnValue(Description = "The method names supported by the server.")]
        string[] mt_supportedMethods();

        [XmlRpcMethod("mt.supportedTextFilters", Description = "The text filters names supported by the server.")]
        [return: XmlRpcReturnValue(Description = "The text filters names supported by the server.")]
        TextFilter[] mt_supportedTextFilters();
    }
}