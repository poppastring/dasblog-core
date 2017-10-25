using System.Web.Caching;

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
    using System.Collections;
    using System.Configuration;
    using System.Web.Configuration;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.IO;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Core;

    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public partial class HomePage : SharedBasePage 
    {

        
        public HomePage()
        {
            
        }
                
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }
		
        private void InitializeComponent()
        {    
            this.Load += new System.EventHandler(this.HomePage_Load);

        }
       
        protected override PlaceHolder ContentPlaceHolder
        {
            get
            {
                return contentPlaceHolder;
            }
        }

        protected override Macros InitializeMacros()
        {
            return MacrosFactory.CreateMacrosInstance( this );
        }

        protected void HomePage_Load(object sender, System.EventArgs e)
		{	
			if ( SiteConfig.EnableStartPageCaching && Request.QueryString != null &&  Request.QueryString["page"] != "admin")
			{
				hideAdminTools = true;                                 
				double seconds = 86400; //max 86400 seconds = 24 hours
				// only the default site without any querys will have an expiration
				// time in seconds to the end of day, with query the site will expire
				// after 24 hours
				if ( Request.QueryString != null && Request.QueryString.Count == 0 )
				{
					DateTime time = DateTime.Now;
					DateTime timeEnd = new DateTime (time.Year , time.Month, time.Day, 23, 59, 59);
					TimeSpan endTimeSpan = new TimeSpan (timeEnd.Ticks - time.Ticks );
					seconds = endTimeSpan.Hours * 60 + endTimeSpan.Minutes * 60 + endTimeSpan.Seconds;
				}
				Response.Cache.SetExpires(DateTime.Now.AddSeconds(seconds)); 
				Response.Cache.VaryByParams["*"]=true;
				Response.Cache.VaryByHeaders["Accept-Language"]=true;
				Response.Cache.SetCacheability(HttpCacheability.Public);
				Response.Cache.SetValidUntilExpires(true);
                
				// insert something into the cache so that we can invalidate it when a new entry or comment is made
				this.DataCache.Insert("BlogCoreData","BlogCoreData",System.Web.Caching.CacheItemPriority.NotRemovable);
				
                Response.AddCacheItemDependency("BlogCoreData");		
				Response.AddFileDependency(SiteConfig.GetConfigFilePathFromCurrentContext());
			}
		}

    }
}
