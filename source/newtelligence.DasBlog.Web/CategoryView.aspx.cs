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
using System.Web;
using System.Web.UI.WebControls;

using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class CategoryView : SharedBasePage
	{
		protected override PlaceHolder ContentPlaceHolder
		{
			get { return contentPlaceHolder; }
		}

		protected override EntryCollection LoadEntries()
		{
			SharedBasePage requestPage = Page as SharedBasePage;

			EntryCollection entryCollection = DataService.GetEntriesForCategory(categoryName, Request.Headers["Accept-Language"]);

			if (!requestPage.SiteConfig.CategoryAllEntries)
			{
				// Do some paging if we are NOT showing all entries in category view.
				string pageString = Request.QueryString.Get("page");
				int page;
				if (!int.TryParse(pageString, out page))
				{
					page = 1;
				}

				if (page != 0)
				{
					int count = entryCollection.Count;
					if (count == 0)
					{
						Response.Redirect(SiteUtilities.GetBaseUrl());
					}

					int entriesPerPage = SiteConfig.EntriesPerPage;
					int maxPage = (int) Math.Ceiling((double) count / (double) entriesPerPage);
					if (page > maxPage)
					{
						page = maxPage;
					}
					int lastEntryToInclude = page * entriesPerPage;

					// page = 1, remove everything after.
					// page = maxPage, remove everything before.
					// 1 < page < maxPage, remove left and right.
					if (page == maxPage)
					{
						entryCollection.RemoveRange(0, lastEntryToInclude - entriesPerPage);
					}
					else
					{
						// Remove to the right.
						entryCollection.RemoveRange(lastEntryToInclude, count - lastEntryToInclude);
						// Remove to the left.
						entryCollection.RemoveRange(0, lastEntryToInclude - entriesPerPage);
					}

					HttpContext.Current.Items["page"] = page;
					HttpContext.Current.Items["maxpage"] = maxPage;
				}
			}

			return entryCollection;
		}

		protected override Macros InitializeMacros()
		{
			return MacrosFactory.CreateMacrosInstance(this);
		}
	}
}