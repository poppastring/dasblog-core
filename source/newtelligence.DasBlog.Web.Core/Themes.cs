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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web.Core
{
	public class BlogTheme
	{
		public const string DefaultTheme = "default";

        private Hashtable imageList = new Hashtable();
		private string name;
		private string title;
		private string templateDirectory;
		private string imageDirectory;
		
		public string Name 
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string Title 
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		public string TemplateDirectory 
		{
			get
			{
				return templateDirectory;
			}
			set
			{
				templateDirectory = value;
			}
		}

		public string ImageDirectory 
		{
			get
			{
				return imageDirectory;
			}
			set
			{
				imageDirectory = value;
			}
		}

        public Hashtable ImageList
        {
            get
            {
                return imageList;
            }
        }

		public static ThemeDictionary Load(string path)
		{
			ThemeDictionary configData = new ThemeDictionary();

			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			foreach (DirectoryInfo info in directoryInfo.GetDirectories())
			{
				FileInfo[] files = info.GetFiles("theme.manifest");
				if (files.Length != 0)
				{
					try
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(files[0].FullName);

						XmlNodeList ndNodeList = doc.GetElementsByTagName("theme");
						foreach( XmlElement elTheme in ndNodeList  )
						{
							BlogTheme blogTheme = new BlogTheme();
							blogTheme.Name = elTheme.GetAttribute("name");
							blogTheme.Title = elTheme.GetAttribute("title");
							blogTheme.TemplateDirectory = elTheme.GetAttribute("templateDirectory");
							blogTheme.ImageDirectory = elTheme.GetAttribute("imageDirectory");
							foreach( XmlElement elImage in elTheme.GetElementsByTagName("image")  )
							{
								string name = elImage.GetAttribute("name");
								string fileName = elImage.GetAttribute("fileName");
								if ( name != null && fileName != null )
								{
									blogTheme.ImageList.Add( name, fileName );
								}
							}
							configData.Add( blogTheme.Name, blogTheme );
						}
					}
					catch (Exception e)
					{
						ErrorTrace.Trace(TraceLevel.Error, e);
					}
				}
			}

			return configData;
		}

		protected StringReader GetCachedTemplate(string key, string filePath)
		{
            DataCache cache = CacheFactory.GetCache();


			string cachedtemplate = cache[key] as string;
			if (cachedtemplate == null || cachedtemplate.Length == 0)
			{
				if (filePath == null) return null;

				using (StreamReader reader = new StreamReader( filePath,  true ))
				{
					cachedtemplate = reader.ReadToEnd();	
				}
				cache.Insert(key,cachedtemplate,new System.Web.Caching.CacheDependency(filePath));
			}
			return new StringReader(cachedtemplate);
		}

        protected TextReader OpenTemplate( string templateName, string basePath, string categoryName)
        {
            string radioStyleTemplate = "#"+templateName+".txt";
            string dasBlogStyleTemplate = templateName+".blogtemplate";
            string htmlStyleTemplate = templateName+".html";

            string sanitizedCategoryName = categoryName.Replace("|","\\");
            sanitizedCategoryName = sanitizedCategoryName.Replace(".","");

			//SDH: Short circuit ALL this logic if we can!
			string key = string.Format("{0}::{1}::{2}::{3}",templateName, basePath, categoryName, this.name);
			TextReader template = null;
			template = GetCachedTemplate(key, null);
			if (template != null) return template;
			
            // try blogStyle with category name first
            string filePath = Path.Combine(Path.Combine(Path.Combine(basePath,this.TemplateDirectory),sanitizedCategoryName),dasBlogStyleTemplate);
            if ( File.Exists( filePath ) )
            {
                template = GetCachedTemplate(key, filePath);
            }
            else 
            {
                // now without category name
                filePath = Path.Combine(Path.Combine(basePath,this.TemplateDirectory),dasBlogStyleTemplate);
                if ( File.Exists( filePath ) )
                {
                    template = GetCachedTemplate(key, filePath);
                }
            }
			
			if (template == null)
			{
				// try html with category name now
				filePath = Path.Combine(Path.Combine(Path.Combine(basePath,this.TemplateDirectory),sanitizedCategoryName),htmlStyleTemplate);
				if ( File.Exists( filePath ) )
				{
					template = GetCachedTemplate(key, filePath);
				}
				else 
				{
					// now without category name
					filePath = Path.Combine(Path.Combine(basePath,this.TemplateDirectory),htmlStyleTemplate);
					if ( File.Exists( filePath ) )
					{
						template = GetCachedTemplate(key, filePath);
					}
				}
			}

			if (template == null)
			{
				// now the same for radio templates
				filePath = Path.Combine(Path.Combine(Path.Combine(basePath,this.TemplateDirectory),sanitizedCategoryName),radioStyleTemplate);
				if ( File.Exists( filePath ) )
				{
					template = GetCachedTemplate(key, filePath);
				}
				else 
				{
					// now without category name
					filePath = Path.Combine(Path.Combine(basePath,this.TemplateDirectory),radioStyleTemplate);
					if ( File.Exists( filePath ) )
					{
						template = GetCachedTemplate(key, filePath);
					}
				}
			}

            return template;
        }

        protected TextReader OpenEmptyTemplate()
        {
            return new StringReader("<p>ERROR: No template found. Check the paths in your theme.manifest and ensure the theme specified in your site.config as the default actually exists!</p>");
        }

		public TextReader OpenMainTemplate(string basePath, string categoryName)
		{
            TextReader templateReader = OpenTemplate("template", basePath, categoryName );
            if ( templateReader == null )
            {
                templateReader = OpenEmptyTemplate();
            }
			return templateReader;
		}

        public TextReader OpenHomeTemplate(string basePath, string categoryName)
		{
			TextReader templateReader = OpenTemplate("homeTemplate", basePath, categoryName );
            if ( templateReader == null )
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
		}

        public TextReader OpenHomeAMPTemplate(string basePath, string categoryName)
        {
            TextReader templateReader = OpenTemplate("homeAMPTemplate", basePath, categoryName);
            if (templateReader == null)
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
        }

        public TextReader OpenItemAMPTemplate(string basePath, string categoryName)
        {
            TextReader templateReader = OpenTemplate("itemAMPTemplate", basePath, categoryName);
            if (templateReader == null)
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
        }

        public TextReader OpenDesktopTemplate(string basePath, string categoryName)
		{
			TextReader templateReader = OpenTemplate("desktopWebsiteTemplate", basePath, categoryName );
            if ( templateReader == null )
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
		}

		public TextReader OpenItemTemplate(string basePath, string categoryName)
		{
			TextReader templateReader = OpenTemplate("itemTemplate", basePath, categoryName );
            if ( templateReader == null )
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
		}

		public TextReader OpenDayTemplate(string basePath, string categoryName)
		{
			TextReader templateReader = OpenTemplate("dayTemplate", basePath, categoryName );
            if ( templateReader == null )
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
		}

        public TextReader OpenDayAMPTemplate(string basePath, string categoryName)
        {
            TextReader templateReader = OpenTemplate("dayAMPTemplate", basePath, categoryName);
            if (templateReader == null)
            {
                templateReader = OpenEmptyTemplate();
            }
            return templateReader;
        }

    }

	/// <summary>
	/// A dictionary with keys of type string and values of type Theme
	/// </summary>
	public class ThemeDictionary: SortedList<string, BlogTheme>
	{
		/// <summary>
		/// Initializes a new empty instance of the ThemeDictionary class
		/// </summary>
		public ThemeDictionary() :base(StringComparer.InvariantCultureIgnoreCase)
		{
			// empty
		}
	}
}
