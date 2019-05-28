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
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Services.XmlRpc.MetaWeblog
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Post
    {
        /// <summary>Date Created in ISO 8601 format</summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public DateTime dateCreated;

        /// <summary></summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public string description;

        /// <summary></summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public string title;

        [XmlRpcMember]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string[] categories;

        /*[XmlRpcMember][XmlRpcMissingMapping(MappingAction.Ignore)]
        public Enclosure enclosure;*/

        /// <summary></summary>
        [XmlRpcMember]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string link;

        /// <summary></summary>
        [XmlRpcMember]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string permalink;

        [XmlRpcMember(
          Description = "Not required when posting. Depending on server may "
          + "be either string or integer. "
          + "Use Convert.ToInt32(postid) to treat as integer or "
          + "Convert.ToString(postid) to treat as string")]
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string postid;


        /// <summary>Allows comment on this post.  Defined as a string because
        /// not all clients send this parameter.  Need to determine when it is 
        /// not sent so that can properly perform valid default behavior.  String
        /// is null when not sent.  Otherwise, client sends an int value.
        ///  0 - None   - Unsure what this means.  Will make this equivalent to Closed for now
        ///  1 - Open   - Allow comments
        ///  2 - Closed - No comments allowed
        /// </summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string mt_allow_comments;

        /// <summary>Currently unused.  DasBlog doesn't allow trackbacks to be turned off on a post by post basis.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public int mt_allow_pings;

        /// <summary>Currently unused.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string mt_convert_breaks;

        /// <summary>Currently unused.  DasBlog only allows an excerpt to be specified.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string mt_text_more;

        /// <summary>The short description for the post that is used in some feeds and can be turned on for the main page.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string mt_excerpt;

        /// <summary>Currently unused.  DasBlog doesn't allow keywords to be specified, only categories.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string mt_keywords;

        /// <summary>Array of trackback URL's to ping.</summary>
        [XmlRpcMember, XmlRpcMissingMapping(MappingAction.Ignore)]
        public string[] mt_tb_ping_urls;

        /// <summary>
        /// Creates a <see cref="MetaWeblog.Post"/> from an <see cref="Entry"/>.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        //public static Post Create(Entry entry)
        //{
        //    if (entry == null) throw new ArgumentNullException("entry");

        //    Post post = new Post();
        //    post.description = entry.Content ?? "";
        //    post.mt_excerpt = entry.Description ?? "";
        //    post.dateCreated = entry.CreatedUtc;
        //    post.title = entry.Title ?? "";
        //    post.link = post.permalink = SiteUtilities.GetPermaLinkUrl(entry);
        //    post.postid = entry.EntryId ?? "";
        //    post.categories = entry.GetSplitCategories();
        //    return post;
        //}
    }
}
