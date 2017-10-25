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
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for PermaLink.
	/// </summary>
	public partial class PermaLink : SharedBasePage
	{

		public PermaLink()
		{
			showTrackingDetail = true;
			isAggregatedView = false;
		}

		protected override PlaceHolder ContentPlaceHolder
		{
			get { return contentPlaceHolder; }
		}

		public override string GetPageTemplate(string path)
		{
			return base.GetPageTemplate(path);
		}

		protected override Macros InitializeMacros()
		{
			return MacrosFactory.CreateMacrosInstance(this);
		}

		public override void ProcessItemTemplate(Entry item, Control ContentPlaceHolder)
		{
			base.ProcessItemTemplate(item, ContentPlaceHolder);

			// add the trackback and pingback information
			string metaData = "";

			if (!this.IsAggregatedView)
			{
				if (this.SiteConfig.EnablePingbackService)
				{
					metaData = string.Format("<link ref=\"pingback\" href=\"{0}\">\n", new Uri(new Uri(this.SiteConfig.Root), "pingback.aspx"));
				}

				if (this.SiteConfig.EnableTrackbackService)
				{
					// we need to ensure that the URL we use in the identifier
					// is the same one used by the remote server or they will
					// not post a trackback since dasBlog supports many url
					// formats.
					string link = Request.Url.GetLeftPart(UriPartial.Authority) + Request.RawUrl.ToString();

					metaData += "\n<!--\n<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"\nxmlns:dc=\"http://purl.org/dc/elements/1.1/\"\nxmlns:trackback=\"http://madskills.com/public/xml/rss/module/trackback/\">\n";
					metaData += string.Format("<rdf:Description\nrdf:about=\"{0}\"\ndc:identifier=\"{1}\"\ndc:title=\"{2}\"\ntrackback:ping=\"{3}\" />\n</rdf:RDF>\n-->\n\n", link, link, item.Title, SiteUtilities.GetTrackbackUrl(item.EntryId));
				}
			}

			ContentPlaceHolder.Controls.Add(new LiteralControl(metaData));
		}

		public override string GetItemTemplate()
		{
			return base.GetItemTemplate();
		}

    
		protected override EntryCollection LoadEntries()
		{
			if (WeblogEntryId.Length == 0) 
			{
				Response.StatusCode = 404;
				Response.SuppressContent = true;
				Response.End();
				return null; //save us all the time	
			}


			EntryCollection entryCollection = new EntryCollection();
			Entry entry = DataService.GetEntry(WeblogEntryId);
            ILoggingDataService logService = this.LoggingService;
			if (entry != null)
			{
				entryCollection.Add(entry);

				if (NotModified(entryCollection))
				{
					Response.End();
					return null;
				}
			}
			
			return entryCollection;
		}

		#region Web Form Designer generated code
		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
