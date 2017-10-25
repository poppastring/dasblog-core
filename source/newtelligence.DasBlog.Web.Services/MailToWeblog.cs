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
using System.IO;
using System.Threading;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using Lesnikowski.Pawel.Mail.Pop3;
using System.Collections;
using System.Text.RegularExpressions;
using newtelligence.DasBlog.Web.Core;
using System.Drawing;
using System.Drawing.Imaging;
using Attachment = Lesnikowski.Pawel.Mail.Pop3.Attachment;

namespace newtelligence.DasBlog.Web.Services
{
    /// <summary>
    /// This is the handler class for the Mail-To-Weblog functionality.
    /// </summary>
    public class MailToWeblog
    {
        string configPath;
        string contentPath;
        string binariesPath;
        string logPath;
        Uri binariesBaseUri;

        public MailToWeblog(string configPath, string contentPath, string binariesPath, string logPath, Uri binariesBaseUri )
        {
            this.configPath = configPath;
            this.contentPath = contentPath;
            this.binariesPath = binariesPath;
            this.logPath = logPath;
            this.binariesBaseUri = binariesBaseUri;
        }

        /// <summary>
        /// Compares two binary buffers up to a certain length.
        /// </summary>
        /// <param name="buf1">First buffer</param>
        /// <param name="buf2">Second buffer</param>
        /// <param name="len">Length</param>
        /// <returns>true or false indicator about the equality of the buffers</returns>
        private bool EqualBuffers( byte[] buf1, byte[] buf2, int len )
        {
            if ( buf1.Length >= len && buf2.Length >= len )
            {
                for( int l=0;l<len;l++)
                {
                    if ( buf1[l]!=buf2[l])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stores an attachment to disk.
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="binariesPath"></param>
        /// <returns></returns>
        public string StoreAttachment( Attachment attachment, string binariesPath )
        {
            bool alreadyUploaded = false;
            string baseFileName = attachment.fileName;
            string targetFileName = Path.Combine(binariesPath,baseFileName);
            int numSuffix=1;
                            
            // if the target filename already exists, we check whether we already 
            // have that file stored by comparing the first 2048 bytes of the incoming
            // date to the target file (creating a hash would be better, but this is 
            // "good enough" for the time being)
            while ( File.Exists(targetFileName))
            {
                byte[] targetBuffer=new byte[Math.Min(2048,attachment.data.Length)];
                int targetBytesRead;

                using(FileStream targetFile = new FileStream(targetFileName,FileMode.Open,FileAccess.Read,FileShare.Read))
                {
                    long numBytes = targetFile.Length;
                    if ( numBytes == (long)attachment.data.Length )
                    {
                        targetBytesRead = targetFile.Read(targetBuffer,0,targetBuffer.Length);
                        if ( targetBytesRead == targetBuffer.Length )
                        {
                            if ( EqualBuffers(targetBuffer, attachment.data, targetBuffer.Length ) )
                            {
                                alreadyUploaded=true;
                            }
                        }
                    }
                }

                // If the file names are equal, but it's not considered the same file,
                // we append an incrementing numeric suffix to the file name and retry.
                if ( !alreadyUploaded )
                {
                    string ext = Path.GetExtension(baseFileName);
                    string file = Path.GetFileNameWithoutExtension(baseFileName);
                    string newFileName = file + (numSuffix++).ToString();
                    baseFileName = newFileName+ext;
                    targetFileName = Path.Combine(binariesPath,baseFileName);
                }
                else
                {
                    break;
                }
            }
                                            
            // now we've got a unique file name or the file is already stored. If it's
            // not stored, write it to disk.
            if ( !alreadyUploaded )
            {
                using(FileStream fileStream = new FileStream(targetFileName,FileMode.Create))
                {
                    fileStream.Write(attachment.data,0,attachment.data.Length);
                    fileStream.Flush();
                }
            }
            return baseFileName;
        }


        /// <summary>
        /// This function is used for thumbnailing and gets an image encoder
        /// for a given mime type, such as image/jpeg
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs) 
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// Mail-To-Weblog runs in background thread and this is the thread function.
        /// </summary>
        public void Run()
        {
            IBlogDataService dataService = null;
            ILoggingDataService loggingService = null;

            SiteConfig siteConfig = SiteConfig.GetSiteConfig( configPath );
            loggingService = LoggingDataServiceFactory.GetService(logPath);
            dataService = BlogDataServiceFactory.GetService(contentPath, loggingService );
            
                    
            ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info,"MailToWeblog thread spinning up");

            loggingService.AddEvent( new EventDataItem( EventCodes.Pop3ServiceStart,"",""));

            do
            {
                try
                {
                    // reload on every cycle to get the current settings
                    siteConfig = SiteConfig.GetSiteConfig( configPath );
                    loggingService = LoggingDataServiceFactory.GetService(logPath);
                    dataService = BlogDataServiceFactory.GetService(contentPath, loggingService);
                    

                    if ( siteConfig.EnablePop3 && 
                        siteConfig.Pop3Server != null && siteConfig.Pop3Server.Length > 0 && 
                        siteConfig.Pop3Username != null && siteConfig.Pop3Username.Length > 0  )
                    {
                        Pop3 pop3=new Pop3();
                        
                        
                        try
                        {
                            pop3.host=siteConfig.Pop3Server;
                            pop3.userName=siteConfig.Pop3Username;
                            pop3.password=siteConfig.Pop3Password;

                            pop3.Connect();
                            pop3.Login();
                            pop3.GetAccountStat();

                            for (int j=pop3.messageCount;j>=1;j--)
                            {
                                Pop3Message message=pop3.GetMessage(j);

                                string messageFrom;
								// luke@jurasource.co.uk 1-MAR-04
								// only delete those messages that are processed
								bool messageWasProcessed = false; 

                                // E-Mail addresses look usually like this:
                                // My Name <myname@example.com> or simply
                                // myname@example.com. This block handles 
                                // both variants.
                                Regex getEmail = new Regex(".*\\<(?<email>.*?)\\>.*");
                                Match matchEmail = getEmail.Match(message.from);
                                if ( matchEmail.Success )
                                {
                                    messageFrom = matchEmail.Groups["email"].Value;
                                }
                                else
                                {
                                    messageFrom = message.from;
                                }
                                
                                // Only if the subject of the message is prefixed (case-sensitive) with
                                // the configured subject prefix, we accept the message
                                if ( message.subject.StartsWith(siteConfig.Pop3SubjectPrefix) )
                                {
                                    
                                    Entry entry = new Entry();
                                    entry.Initialize();
                                    entry.Title = message.subject.Substring(siteConfig.Pop3SubjectPrefix.Length);
                                    entry.Categories="";
                                    entry.Content = "";
									entry.Author = messageFrom; //store the email, what we have for now...

                                    // Grab the categories. Categories are defined in square brackets 
                                    // in the subject line.
                                    Regex categoriesRegex = new Regex( "(?<exp>\\[(?<cat>.*?)\\])" );
                                    foreach( Match match in categoriesRegex.Matches( entry.Title ) )
                                    {
                                        entry.Title = entry.Title.Replace(match.Groups["exp"].Value,"");
                                        entry.Categories += match.Groups["cat"].Value+";";
                                    }
                                    entry.Title = entry.Title.Trim();
                                    
                                    string categories = "";
                                    string[] splitted = entry.Categories.Split(';');
                                    for( int i=0;i<splitted.Length;i++)
                                    {
                                        categories += splitted[i].Trim()+";";
                                    }
                                    entry.Categories = categories.TrimEnd(';');

                                    
                                    entry.CreatedUtc = RFC2822Date.Parse(message.date);
                                    
									#region PLain Text
                                    // plain text?
                                    if ( message.contentType.StartsWith("text/plain") )
                                    {
                                        entry.Content += message.body;
                                    }
									#endregion

									#region Just HTML
										// Luke Latimer 16-FEB-2004 (luke@jurasource.co.uk)
										// HTML only emails were not appearing
									else if ( message.contentType.StartsWith("text/html") )
									{
										string messageText = "";

										// Note the email may still be encoded
										//messageText = QuotedCoding.DecodeOne(message.charset, "Q", message.body);										
										messageText = message.body;

										// Strip the <body> out of the message (using code from below)
										Regex bodyExtractor = new Regex("<body.*?>(?<content>.*)</body>",RegexOptions.IgnoreCase|RegexOptions.Singleline);
										Match match = bodyExtractor.Match( messageText );
										if ( match != null && match.Success && match.Groups["content"] != null )
										{
											entry.Content += match.Groups["content"].Value;
										}
										else
										{
											entry.Content += messageText;
										}

									}
										#endregion
				
                                      // HTML/Text with attachments ?
                                    else if ( 
										message.contentType.StartsWith("multipart/alternative")||
										message.contentType.StartsWith("multipart/related") ||
                                        message.contentType.StartsWith("multipart/mixed") )
                                    {
                                        Hashtable embeddedFiles = new Hashtable();
                                        ArrayList attachedFiles = new ArrayList();

                                        foreach( Attachment attachment in message.attachments )
                                        {
                                            // just plain text?
                                            if ( attachment.contentType.StartsWith("text/plain") )
                                            {
                                                entry.Content += StringOperations.GetString(attachment.data);
                                            }

												// Luke Latimer 16-FEB-2004 (luke@jurasource.co.uk)
												// Allow for html-only attachments
											else if ( attachment.contentType.StartsWith("text/html") )
											{
												// Strip the <body> out of the message (using code from below)												
												Regex bodyExtractor = new Regex("<body.*?>(?<content>.*)</body>",RegexOptions.IgnoreCase|RegexOptions.Singleline);
												string htmlString = StringOperations.GetString(attachment.data);
												Match match = bodyExtractor.Match( htmlString );

												//NOTE: We will BLOW AWAY any previous content in this case.
												// This is because most mail clients like Outlook include
												// plain, then HTML. We will grab plain, then blow it away if 
												// HTML is included later.
												if ( match != null && match.Success && match.Groups["content"] != null )
												{
													entry.Content = match.Groups["content"].Value;
												}
												else
												{
													entry.Content = htmlString;
												}
											}			

                                            // or alternative text ?
                                            else if ( attachment.contentType.StartsWith("multipart/alternative") )
                                            {
                                                bool contentSet = false;
                                                string textContent = null;
                                                foreach( Attachment inner_attachment in attachment.attachments )
                                                {
                                                    // we prefer HTML
                                                    if ( inner_attachment.contentType.StartsWith("text/plain") )
                                                    {
                                                        textContent = StringOperations.GetString(inner_attachment.data);
                                                    }
                                                    else if ( inner_attachment.contentType.StartsWith("text/html") )
                                                    {
                                                        Regex bodyExtractor = new Regex("<body.*?>(?<content>.*)</body>", RegexOptions.IgnoreCase|RegexOptions.Singleline);
                                                        string htmlString = StringOperations.GetString(inner_attachment.data);
                                                        Match match = bodyExtractor.Match( htmlString );
                                                        if ( match != null && match.Success && match.Groups["content"] != null )
                                                        {
                                                            entry.Content += match.Groups["content"].Value;
                                                        }
                                                        else
                                                        {
                                                            entry.Content += htmlString;
                                                        }
                                                        contentSet = true;
                                                    }
                                                }
                                                if ( !contentSet )
                                                {
                                                    entry.Content += textContent;
                                                }
                                            }
                                                // or text with embeddedFiles (in a mixed message only)
                                            else if ( (message.contentType.StartsWith("multipart/mixed") || message.contentType.StartsWith("multipart/alternative"))
												&& attachment.contentType.StartsWith("multipart/related") )
                                            {
												foreach( Attachment inner_attachment in attachment.attachments )
                                                {
                                                    // just plain text?
                                                    if ( inner_attachment.contentType.StartsWith("text/plain") )
                                                    {
                                                        entry.Content += StringOperations.GetString(inner_attachment.data);
                                                    }

													else if ( inner_attachment.contentType.StartsWith("text/html") )
                                                    {
                                                        Regex bodyExtractor = new Regex("<body.*?>(?<content>.*)</body>", RegexOptions.IgnoreCase|RegexOptions.Singleline);
                                                        string htmlString = StringOperations.GetString(inner_attachment.data);
                                                        Match match = bodyExtractor.Match( htmlString );
                                                        if ( match != null && match.Success && match.Groups["content"] != null )
                                                        {
                                                            entry.Content += match.Groups["content"].Value;
                                                        }
                                                        else
                                                        {
                                                            entry.Content += htmlString;
                                                        }
                                                    }
   														
													// or alternative text ?
                                                    else if ( inner_attachment.contentType.StartsWith("multipart/alternative") )
                                                    {
                                                        bool contentSet = false;
                                                        string textContent = null;
                                                        foreach( Attachment inner_inner_attachment in inner_attachment.attachments )
                                                        {
                                                            // we prefer HTML
                                                            if ( inner_inner_attachment.contentType.StartsWith("text/plain") )
                                                            {
                                                                textContent = StringOperations.GetString(inner_inner_attachment.data);
                                                            }
                                                            else if ( inner_inner_attachment.contentType.StartsWith("text/html") )
                                                            {
                                                                Regex bodyExtractor = new Regex("<body.*?>(?<content>.*)</body>",RegexOptions.IgnoreCase|RegexOptions.Singleline);
                                                                string htmlString = StringOperations.GetString(inner_inner_attachment.data);
                                                                Match match = bodyExtractor.Match( htmlString );
                                                                if ( match != null && match.Success && match.Groups["content"] != null )
                                                                {
                                                                    entry.Content += match.Groups["content"].Value;
                                                                }
                                                                else
                                                                {
                                                                    entry.Content += htmlString;
                                                                }
                                                                contentSet = true;
                                                            }
                                                        }
                                                        if ( !contentSet )
                                                        {
                                                            entry.Content += textContent;
                                                        }
                                                    }
                                                        // any other inner_attachment
                                                    else if ( inner_attachment.data != null && 
                                                        inner_attachment.fileName != null && 
                                                        inner_attachment.fileName.Length > 0)
                                                    {
                                                        if ( inner_attachment.contentID.Length > 0 )
                                                        {
                                                            embeddedFiles.Add(inner_attachment.contentID, StoreAttachment( inner_attachment, binariesPath ));
                                                        }
                                                        else
                                                        {
                                                            attachedFiles.Add(StoreAttachment( inner_attachment, binariesPath ));
                                                        }
                                                    }
                                                }
                                            }
                                                // any other attachment
                                            else if ( attachment.data != null && 
                                                attachment.fileName != null && 
                                                attachment.fileName.Length > 0)
                                            {
                                                
                                                if ( attachment.contentID.Length > 0 && message.contentType.StartsWith("multipart/related"))
                                                {
                                                    embeddedFiles.Add(attachment.contentID, StoreAttachment( attachment, binariesPath ));
                                                }
                                                else
                                                {
                                                    attachedFiles.Add(StoreAttachment( attachment, binariesPath ));
                                                }
                                                
                                            }
                                        }


                                        // check for orphaned embeddings
                                        string[] embeddedKeys = new string[embeddedFiles.Keys.Count];
                                        embeddedFiles.Keys.CopyTo(embeddedKeys,0);
                                        foreach( string key in embeddedKeys )
                                        {
                                            if ( entry.Content.IndexOf("cid:"+key.Trim('<','>')) == -1 )
                                            {
                                                object file = embeddedFiles[key];
                                                embeddedFiles.Remove(key);
                                                attachedFiles.Add(file);
                                            }
                                        }
                                        
                                        
                                        // now fix up the URIs
                                        
                                        if ( siteConfig.Pop3InlineAttachedPictures )
                                        {
                                            foreach( string fileName in attachedFiles )
                                            {
                                                string fileNameU = fileName.ToUpper();
                                                if ( fileNameU.EndsWith(".JPG") || fileNameU.EndsWith(".JPEG") ||
                                                    fileNameU.EndsWith(".GIF") || fileNameU.EndsWith(".PNG") ||
                                                    fileNameU.EndsWith(".BMP") )
                                                {
                                                    bool scalingSucceeded = false;

                                                    if ( siteConfig.Pop3InlinedAttachedPicturesThumbHeight > 0 )
                                                    {
                                                        try
                                                        {
                                                            string absoluteFileName = Path.Combine(binariesPath, fileName);
                                                            string thumbBaseFileName = Path.GetFileNameWithoutExtension(fileName)+"-thumb.dasblog.JPG";
                                                            string thumbFileName = Path.Combine(binariesPath, thumbBaseFileName);
                                                            Bitmap sourceBmp = new Bitmap(absoluteFileName);
                                                            if ( sourceBmp.Height > siteConfig.Pop3InlinedAttachedPicturesThumbHeight )
                                                            {
                                                                Bitmap targetBmp = new Bitmap(sourceBmp,new Size(
                                                                    Convert.ToInt32(Math.Round((((double)sourceBmp.Width)*(((double)siteConfig.Pop3InlinedAttachedPicturesThumbHeight)/((double)sourceBmp.Height))),0)),
                                                                    siteConfig.Pop3InlinedAttachedPicturesThumbHeight));
                                                                
                                                                ImageCodecInfo codecInfo = GetEncoderInfo("image/jpeg");
                                                                Encoder encoder = Encoder.Quality;
                                                                EncoderParameters encoderParams= new EncoderParameters(1);
                                                                long compression=75;
                                                                EncoderParameter encoderParam = new EncoderParameter(encoder,compression);
                                                                encoderParams.Param[0] = encoderParam;
                                                                targetBmp.Save(thumbFileName,codecInfo,encoderParams);
                                                                
                                                                string absoluteUri = new Uri( binariesBaseUri, fileName ).AbsoluteUri;
                                                                string absoluteThumbUri = new Uri( binariesBaseUri, thumbBaseFileName ).AbsoluteUri;
                                                                entry.Content += String.Format("<div class=\"inlinedMailPictureBox\"><a href=\"{0}\"><img border=\"0\" class=\"inlinedMailPicture\" src=\"{2}\"></a><br /><a class=\"inlinedMailPictureLink\" href=\"{0}\">{1}</a></div>",absoluteUri, fileName, absoluteThumbUri);
                                                                scalingSucceeded = true;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                    if ( !scalingSucceeded )
                                                    {
                                                        string absoluteUri = new Uri( binariesBaseUri, fileName ).AbsoluteUri;
                                                        entry.Content += String.Format("<div class=\"inlinedMailPictureBox\"><img class=\"inlinedMailPicture\" src=\"{0}\"><br /><a class=\"inlinedMailPictureLink\" href=\"{0}\">{1}</a></div>",absoluteUri, fileName);
                                                    }
                                                }
                                            }
                                        }

                                        if ( attachedFiles.Count > 0 )
                                        {
                                            entry.Content += "<p>";
                                        }

                                        foreach( string fileName in attachedFiles )
                                        {
                                            string fileNameU = fileName.ToUpper();
                                            if ( !siteConfig.Pop3InlineAttachedPictures ||
                                                ( !fileNameU.EndsWith(".JPG") && !fileNameU.EndsWith(".JPEG") &&
                                                !fileNameU.EndsWith(".GIF") && !fileNameU.EndsWith(".PNG") &&
                                                !fileNameU.EndsWith(".BMP") ))
                                            {
                                                string absoluteUri = new Uri( binariesBaseUri, fileName ).AbsoluteUri;
                                                entry.Content += String.Format("Download: <a href=\"{0}\">{1}</a><br />",absoluteUri, fileName);
                                            }
                                        }
                                        if ( attachedFiles.Count > 0 )
                                        {
                                            entry.Content += "</p>";
                                        }
                                        
                                        foreach( string key in embeddedFiles.Keys )
                                        {
                                            entry.Content = entry.Content.Replace("cid:"+key.Trim('<','>'), new Uri( binariesBaseUri, (string)embeddedFiles[key] ).AbsoluteUri );
                                        }
                                    }

									loggingService.AddEvent(
										new EventDataItem( 
										EventCodes.Pop3EntryReceived, entry.Title, 
										SiteUtilities.GetPermaLinkUrl(siteConfig,entry.EntryId),messageFrom));

                                    SiteUtilities.SaveEntry(entry, siteConfig, loggingService, dataService);
                                    
                                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info,
                                        String.Format("Message stored. From: {0}, Title: {1} as entry {2}",
                                        messageFrom, entry.Title, entry.EntryId ));

                                    // give the XSS upstreamer a hint that things have changed
//                                    XSSUpstreamer.TriggerUpstreaming();
									
									// luke@jurasource.co.uk (01-MAR-04)
									messageWasProcessed = true;

                                }
                                else
                                {
									// luke@jurasource.co.uk (01-MAR-04)
									// logging every ignored email is apt
									// to fill up the event page very quickly
									// especially if only processed emails are
									// being deleted
									if ( siteConfig.Pop3LogIgnoredEmails )
									{
										loggingService.AddEvent(
											new EventDataItem( 
											EventCodes.Pop3EntryIgnored, message.subject, 
											null, messageFrom));
									}
                                }
								// luke@jurasource.co.uk (01-MAR-04)
								if ( siteConfig.Pop3DeleteAllMessages || messageWasProcessed )
								{
									if (!messageWasProcessed)
									{
										loggingService.AddEvent(
											new EventDataItem( 
											EventCodes.Pop3EntryDiscarded, message.subject, 
											null, messageFrom));
									}
									pop3.DeleteMessage(j);
								}
                            }
                            
                        }
                        catch( Exception e )
                        {
                            ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
                            loggingService.AddEvent(
                                new EventDataItem( 
                                EventCodes.Pop3ServerError, e.ToString().Replace("\n","<br />"), null, null));
                        }
                        finally
                        {
                            pop3.Close();
                        }
                    }               

                    Thread.Sleep( TimeSpan.FromSeconds( siteConfig.Pop3Interval ) );
                }
                catch( ThreadAbortException abortException )
                {
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info,abortException);
                    loggingService.AddEvent( new EventDataItem( EventCodes.Pop3ServiceShutdown,"",""));
                    break;
                }
                catch( Exception e )
                {
                    // if the siteConfig can't be read, stay running regardless 
                    // default wait time is 4 minutes in that case
                    Thread.Sleep( TimeSpan.FromSeconds(240));
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
                }
                
            }
            while ( true );
            ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info,"MailToWeblog thread terminating");
            loggingService.AddEvent( new EventDataItem( EventCodes.Pop3ServiceShutdown,"",""));
        }
    }
}
