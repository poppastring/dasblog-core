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

using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace newtelligence.DasBlog.Web.Core
{
    /// <summary>
    /// This form class is injected into the template
    /// </summary>
	public class BaseHtmlForm : HtmlForm
	{
        /// <summary>
        /// This overriden method fixes the action= attribute of the form 
        /// to point to the proper page.
        /// </summary>
        /// <param name="writer"></param>
		protected override void RenderAttributes(HtmlTextWriter writer)
		{			
			StringWriter sw = new StringWriter();
			HtmlTextWriter htw = new HtmlTextWriter(sw);
			string searchKey = "action=";

			base.RenderAttributes( htw );

			string attributes = sw.GetStringBuilder().ToString();
			int actionIndex = attributes.IndexOf(searchKey);
			if ( actionIndex >= 0 )
			{
				int endIndex, startIndex = actionIndex + searchKey.Length;
				if ( attributes[startIndex] == '\"' )
				{
					startIndex += 1;
					endIndex = attributes.IndexOf("\"",startIndex);
				}
				else
				{
					endIndex = attributes.IndexOf(" ",startIndex);
				}
				if ( endIndex == -1 )
				{
					endIndex = attributes.Length-1;
				}
				attributes = attributes.Remove(startIndex,endIndex-startIndex);
                // htmlencode to make the action attribute xHtml compatible
				attributes = attributes.Insert(startIndex, HttpUtility.HtmlAttributeEncode(this.Page.Request.Url.PathAndQuery ));
			}
			writer.Write( attributes );
		}
	}
}
