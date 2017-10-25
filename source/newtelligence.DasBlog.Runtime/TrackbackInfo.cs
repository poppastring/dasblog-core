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

namespace newtelligence.DasBlog.Runtime
{
    [Serializable]
    public class TrackbackInfo
    {
        //bool autoTrackback;
        string targetUrl;
        string sourceUrl;
        string sourceTitle;
        string sourceExcerpt;
        string sourceBlogName;
        
		/*
		/// <summary>
		/// Creates a new Trackback with AutoTrackback
		/// </summary>
		/// <param name="sourceUrl"></param>
		/// <param name="sourceTitle"></param>
		/// <param name="sourceExcerpt"></param>
		/// <param name="sourceBlogName"></param>
        public TrackbackInfo(string sourceUrl, string sourceTitle, string sourceExcerpt, string sourceBlogName)
        {
            autoTrackback = true;
            this.sourceBlogName = sourceBlogName;
            this.sourceExcerpt=sourceExcerpt;
            this.sourceTitle=sourceTitle;
            this.sourceUrl=sourceUrl;
        }
		*/

		/// <summary>
		/// Creates a new Trackback
		/// </summary>
		/// <param name="targetUrl"></param>
		/// <param name="sourceUrl"></param>
		/// <param name="sourceTitle"></param>
		/// <param name="sourceExcerpt"></param>
		/// <param name="sourceBlogName"></param>
        public TrackbackInfo(string targetUrl, string sourceUrl, string sourceTitle, string sourceExcerpt, string sourceBlogName)
        {
            //autoTrackback = false;
            this.targetUrl = targetUrl;
            this.sourceBlogName = sourceBlogName;
            this.sourceExcerpt=sourceExcerpt;
            this.sourceTitle=sourceTitle;
            this.sourceUrl=sourceUrl;
        }
        public string TargetUrl { get { return targetUrl; } set { targetUrl = value; }}
        public string SourceUrl { get { return sourceUrl; } set { sourceUrl = value; }}
        public string SourceTitle { get { return sourceTitle; } set { sourceTitle = value; }}
        public string SourceExcerpt { get { return sourceExcerpt; } set { sourceExcerpt = value; }}
        public string SourceBlogName { get { return sourceBlogName; } set { sourceBlogName = value; }}
	    //public bool AutoTrackback { get{ return autoTrackback; }}
    }
}
