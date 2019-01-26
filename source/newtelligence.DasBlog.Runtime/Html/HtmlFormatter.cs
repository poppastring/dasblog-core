#region Copyright (c) 2003, newtelligence AG. All rights reserved.
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
// -------
// Copyright 2003, Microsoft Coporation
//
// Original source code by Nikhil Kothari
// 
// Integrated into DasBlog by Chris Anderson
//
//   Provided as is, with no warrenty, etc.
//   License is granted to use, copy, modify, 
//   with or without credit to me, just don't
//   blame me if it doesn't work.
// -------
#endregion

using newtelligence.DasBlog.Runtime.Html.Formatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace newtelligence.DasBlog.Runtime.Util.Html
{
    /// <summary>
    /// </summary>
    /// TODO: Have the formatter take a TextBuffer and format that buffer and output a string
    /// TODO: Handle TD tags correctly
    public sealed class HtmlFormatter
    {
        private static IDictionary<string, TagInfo> tagTable;

        //private delegate bool CanContainTag(TagInfo info);

        private static TagInfo commentTag;
        private static TagInfo directiveTag;
        private static TagInfo otherServerSideScriptTag;
        private static TagInfo nestedXmlTag;
        private static TagInfo unknownXmlTag;
        private static TagInfo unknownHtmlTag;

        static HtmlFormatter()
        {
            commentTag = new TagInfo("", FormattingFlags.Comment | FormattingFlags.NoEndTag, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, ElementType.Any);
            directiveTag = new TagInfo("", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            otherServerSideScriptTag = new TagInfo("", FormattingFlags.NoEndTag | FormattingFlags.Inline, ElementType.Any);
            unknownXmlTag = new TagInfo("", FormattingFlags.Xml, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            nestedXmlTag = new TagInfo("", FormattingFlags.AllowPartialTags, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            unknownHtmlTag = new TagInfo("", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);

            tagTable = new Dictionary<string, TagInfo>(StringComparer.OrdinalIgnoreCase);

            tagTable["a"] = new TagInfo("a", FormattingFlags.Inline, ElementType.Inline);
            tagTable["acronym"] = new TagInfo("acronym", FormattingFlags.Inline, ElementType.Inline);
            tagTable["address"] = new TagInfo("address", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["applet"] = new TagInfo("applet", FormattingFlags.Inline, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["area"] = new TagInfo("area", FormattingFlags.NoEndTag);
            tagTable["b"] = new TagInfo("b", FormattingFlags.Inline, ElementType.Inline);
            tagTable["base"] = new TagInfo("base", FormattingFlags.NoEndTag);
            tagTable["basefont"] = new TagInfo("basefont", FormattingFlags.NoEndTag, ElementType.Block);
            tagTable["bdo"] = new TagInfo("bdo", FormattingFlags.Inline, ElementType.Inline);
            tagTable["bgsound"] = new TagInfo("bgsound", FormattingFlags.NoEndTag);
            tagTable["big"] = new TagInfo("big", FormattingFlags.Inline, ElementType.Inline);
            tagTable["blink"] = new TagInfo("blink", FormattingFlags.Inline);
            tagTable["blockquote"] = new TagInfo("blockquote", FormattingFlags.Inline, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["body"] = new TagInfo("body", FormattingFlags.None);
            tagTable["br"] = new TagInfo("br", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Inline);
            //tagTable["br"] = new TagInfo("br", FormattingFlags.NoEndTag, WhiteSpaceType.Significant, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["button"] = new TagInfo("button", FormattingFlags.Inline, ElementType.Inline);
            tagTable["caption"] = new TagInfo("caption", FormattingFlags.None);
            tagTable["cite"] = new TagInfo("cite", FormattingFlags.Inline, ElementType.Inline);
            tagTable["center"] = new TagInfo("center", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["code"] = new TagInfo("code", FormattingFlags.Inline, ElementType.Inline);
            tagTable["col"] = new TagInfo("col", FormattingFlags.NoEndTag);
            tagTable["colgroup"] = new TagInfo("colgroup", FormattingFlags.None);
            tagTable["dd"] = new TagInfo("dd", FormattingFlags.None);
            tagTable["del"] = new TagInfo("del", FormattingFlags.None);
            tagTable["dfn"] = new TagInfo("dfn", FormattingFlags.Inline, ElementType.Inline);
            tagTable["dir"] = new TagInfo("dir", FormattingFlags.None, ElementType.Block);
            tagTable["div"] = new TagInfo("div", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["dl"] = new TagInfo("dl", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["dt"] = new TagInfo("dt", FormattingFlags.Inline);
            tagTable["em"] = new TagInfo("em", FormattingFlags.Inline, ElementType.Inline);
            tagTable["embed"] = new TagInfo("embed", FormattingFlags.Inline, WhiteSpaceType.Significant, WhiteSpaceType.CarryThrough, ElementType.Inline);
            tagTable["fieldset"] = new TagInfo("fieldset", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["font"] = new TagInfo("font", FormattingFlags.Inline, ElementType.Inline);
            tagTable["form"] = new TagInfo("form", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["frame"] = new TagInfo("frame", FormattingFlags.NoEndTag);
            tagTable["frameset"] = new TagInfo("frameset", FormattingFlags.None);
            tagTable["head"] = new TagInfo("head", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant);
            tagTable["h1"] = new TagInfo("h1", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h2"] = new TagInfo("h2", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h3"] = new TagInfo("h3", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h4"] = new TagInfo("h4", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h5"] = new TagInfo("h5", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h6"] = new TagInfo("h6", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            // REVIEW: <hr> was changed to be an Block element b/c IE appears to allow it.
            tagTable["hr"] = new TagInfo("hr", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["html"] = new TagInfo("html", FormattingFlags.NoIndent, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant);
            tagTable["i"] = new TagInfo("i", FormattingFlags.Inline, ElementType.Inline);
            tagTable["iframe"] = new TagInfo("iframe", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Inline);
            tagTable["img"] = new TagInfo("img", FormattingFlags.Inline | FormattingFlags.NoEndTag, WhiteSpaceType.Significant, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["input"] = new TagInfo("input", FormattingFlags.NoEndTag, WhiteSpaceType.Significant, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["ins"] = new TagInfo("ins", FormattingFlags.None);
            tagTable["isindex"] = new TagInfo("isindex", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.CarryThrough, ElementType.Block);
            tagTable["kbd"] = new TagInfo("kbd", FormattingFlags.Inline, ElementType.Inline);
            tagTable["label"] = new TagInfo("label", FormattingFlags.Inline, ElementType.Inline);
            tagTable["legend"] = new TagInfo("legend", FormattingFlags.None);
            tagTable["li"] = new LITagInfo();
            tagTable["link"] = new TagInfo("link", FormattingFlags.NoEndTag);
            tagTable["listing"] = new TagInfo("listing", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["map"] = new TagInfo("map", FormattingFlags.Inline, ElementType.Inline);
            tagTable["marquee"] = new TagInfo("marquee", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["menu"] = new TagInfo("menu", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["meta"] = new TagInfo("meta", FormattingFlags.NoEndTag);
            tagTable["nobr"] = new TagInfo("nobr", FormattingFlags.Inline | FormattingFlags.NoEndTag, ElementType.Inline);
            tagTable["noembed"] = new TagInfo("noembed", FormattingFlags.None, ElementType.Block);
            tagTable["noframes"] = new TagInfo("noframes", FormattingFlags.None, ElementType.Block);
            tagTable["noscript"] = new TagInfo("noscript", FormattingFlags.None, ElementType.Block);
            tagTable["object"] = new TagInfo("object", FormattingFlags.None, ElementType.Inline);
            tagTable["ol"] = new OLTagInfo();
            tagTable["option"] = new TagInfo("option", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.CarryThrough);
            tagTable["p"] = new PTagInfo();
            tagTable["param"] = new TagInfo("param", FormattingFlags.NoEndTag);
            tagTable["pre"] = new TagInfo("pre", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Block);
            tagTable["q"] = new TagInfo("q", FormattingFlags.Inline, ElementType.Inline);
            tagTable["rt"] = new TagInfo("rt", FormattingFlags.None);
            tagTable["ruby"] = new TagInfo("ruby", FormattingFlags.None, ElementType.Inline);
            tagTable["s"] = new TagInfo("s", FormattingFlags.Inline, ElementType.Inline);
            tagTable["samp"] = new TagInfo("samp", FormattingFlags.None, ElementType.Inline);
            tagTable["script"] = new TagInfo("script", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, ElementType.Inline);
            tagTable["select"] = new TagInfo("select", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Block);
            tagTable["small"] = new TagInfo("small", FormattingFlags.Inline, ElementType.Inline);
            tagTable["span"] = new TagInfo("span", FormattingFlags.Inline, ElementType.Inline);
            tagTable["strike"] = new TagInfo("strike", FormattingFlags.Inline, ElementType.Inline);
            tagTable["strong"] = new TagInfo("strong", FormattingFlags.Inline, ElementType.Inline);
            tagTable["style"] = new TagInfo("style", FormattingFlags.PreserveContent, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            tagTable["sub"] = new TagInfo("sub", FormattingFlags.Inline, ElementType.Inline);
            tagTable["sup"] = new TagInfo("sup", FormattingFlags.Inline, ElementType.Inline);
            tagTable["table"] = new TagInfo("table", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["tbody"] = new TagInfo("tbody", FormattingFlags.None);
            tagTable["td"] = new TDTagInfo();
            tagTable["textarea"] = new TagInfo("textarea", FormattingFlags.Inline, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["tfoot"] = new TagInfo("tfoot", FormattingFlags.None);
            tagTable["th"] = new TagInfo("th", FormattingFlags.None);
            tagTable["thead"] = new TagInfo("thead", FormattingFlags.None);
            tagTable["title"] = new TagInfo("title", FormattingFlags.Inline);
            tagTable["tr"] = new TRTagInfo();
            tagTable["tt"] = new TagInfo("tt", FormattingFlags.Inline, ElementType.Inline);
            tagTable["u"] = new TagInfo("u", FormattingFlags.Inline, ElementType.Inline);
            tagTable["ul"] = new TagInfo("ul", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["xml"] = new TagInfo("xml", FormattingFlags.Xml, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["xmp"] = new TagInfo("xmp", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["var"] = new TagInfo("var", FormattingFlags.Inline, ElementType.Inline);
            tagTable["wbr"] = new TagInfo("wbr", FormattingFlags.Inline | FormattingFlags.NoEndTag, ElementType.Inline);
        }

        public void Format(string input, TextWriter output, HtmlFormatterOptions options)
        {
            // Determine if we are outputting xhtml
            bool makeXhtml = options.MakeXhtml;

            // Save the max line length
            int maxLineLength = options.MaxLineLength;

            // Make the indent string
            string indentString = new String(options.IndentChar, options.IndentSize);

            char[] chars = input.ToCharArray();
            Stack<FormatInfo> tagStack = new Stack<FormatInfo>();
            Stack<HtmlWriter> writerStack = new Stack<HtmlWriter>();

            // The previous begin or end tag that was seen
            FormatInfo previousTag = null;

            // The current begin or end tag that was seen
            FormatInfo currentTag = null;

            // The text between previousTag and currentTag
            string text = String.Empty;

            // True if we've seen whitespace at the end of the last text outputted
            bool sawWhiteSpace = false;

            // True if the last tag seen was self-terminated with a '/>'
            bool sawSelfTerminatingTag = false;

            // True if we saw a tag that we decided not to render
            bool ignoredLastTag = false;

            // Put the initial writer on the stack
            HtmlWriter writer = new HtmlWriter(output, indentString, maxLineLength);
            writerStack.Push(writer);

            Token t = HtmlTokenizer.GetFirstToken(chars);
            Token lastToken = t;

            while (t != null)
            {
                writer = writerStack.Peek();
                switch (t.Type)
                {
                    case Token.AttrName:
                        if (makeXhtml)
                        {
                            string attrName = String.Empty;
                            if (!previousTag.tagInfo.IsXml)
                            {
                                // Need to lowercase the HTML attribute names for XHTML
                                attrName = t.Text.ToLower();
                            }
                            else
                            {
                                attrName = t.Text;
                            }
                            writer.Write(attrName);

                            // If we are trying to be compliant XHTML, don't allow attribute minimization
                            Token nextToken = HtmlTokenizer.GetNextToken(t);
                            if (nextToken.Type != Token.EqualsChar)
                            {
                                writer.Write("=\"" + attrName + "\"");
                            }
                        }
                        else
                        {
                            // Convert the case of the attribute if the tag isn't xml
                            if (!previousTag.tagInfo.IsXml)
                            {
                                if (options.AttributeCasing == HtmlFormatterCase.UpperCase)
                                {
                                    writer.Write(t.Text.ToUpper());
                                }
                                else if (options.AttributeCasing == HtmlFormatterCase.LowerCase)
                                {
                                    writer.Write(t.Text.ToLower());
                                }
                                else
                                {
                                    writer.Write(t.Text);
                                }
                            }
                            else
                            {
                                writer.Write(t.Text);
                            }
                        }
                        break;
                    case Token.AttrVal:
                        if (makeXhtml && (lastToken.Type != Token.DoubleQuote) && (lastToken.Type != Token.SingleQuote))
                        {
                            // If the attribute value isn't quoted, double quote it, replacing the inner double quotes
                            writer.Write('\"');
                            writer.Write(t.Text.Replace("\"", "&quot;"));
                            writer.Write('\"');
                        }
                        else
                        {
                            writer.Write(t.Text);
                        }
                        break;
                    case Token.CloseBracket:
                        if (makeXhtml)
                        {
                            if (ignoredLastTag)
                            {
                                // Don't render the close bracket if we ignored the last tag
                                ignoredLastTag = false;
                            }
                            else
                            {
                                if (sawSelfTerminatingTag && (!previousTag.tagInfo.IsComment))
                                {
                                    // If we saw a self terminating tag, that doesn't have the forward slash, put it in (except for comments)
                                    writer.Write(" />");
                                }
                                else
                                {
                                    // If we are just closing a normal tag, just put in a normal close bracket
                                    writer.Write('>');
                                }
                            }
                        }
                        else
                        {
                            // If there's no XHTML to be made, just put in a normal close bracket
                            writer.Write('>');
                        }
                        break;
                    case Token.DoubleQuote:
                        writer.Write('\"');
                        break;
                    case Token.Empty:
                        break;
                    case Token.EqualsChar:
                        writer.Write('=');
                        break;
                    case Token.Error:
                        if (lastToken.Type == Token.OpenBracket)
                        {
                            // Since we aren't outputting open brackets right away, we might have to output one now
                            writer.Write('<');
                        }
                        writer.Write(t.Text);
                        break;
                    case Token.ForwardSlash:
                    case Token.OpenBracket:
                        // Just push these symbols on the stack for now... output them when we write the tag
                        break;
                    case Token.SelfTerminating:
                        previousTag.isEndTag = true;
                        if (!previousTag.tagInfo.NoEndTag)
                        {
                            // If the tag that is self-terminating is normally not a self-closed tag
                            // then we've placed an entry on the stack for it.  Since it's self terminating, we now need
                            // to pop that item off of the tag stack
                            tagStack.Pop();

                            // If it was a self-closed Xml tag, then we also need to clean up the writerStack
                            if (previousTag.tagInfo.IsXml)
                            {
                                HtmlWriter oldWriter = writerStack.Pop();
                                writer = writerStack.Peek();

                                // Since a self-closed xml tag can't have any text content, we can just write out the formatted contents
                                writer.Write(oldWriter.Content);
                            }
                        }
                        if ((lastToken.Type == Token.Whitespace) && (lastToken.Text.Length > 0))
                        {
                            writer.Write("/>");
                        }
                        else
                        {
                            writer.Write(" />");
                        }
                        break;
                    case Token.SingleQuote:
                        writer.Write('\'');
                        break;
                    case Token.XmlDirective:
                        writer.WriteLineIfNotOnNewLine();
                        writer.Write('<');
                        writer.Write(t.Text);
                        writer.Write('>');
                        writer.WriteLineIfNotOnNewLine();
                        ignoredLastTag = true;
                        break;
                    case Token.TagName:
                    case Token.Comment:
                    case Token.InlineServerScript:
                        string tagName;

                        // Reset the self terminating tag flag
                        sawSelfTerminatingTag = false;

                        // Create or get the proper tagInfo, depending on the type of token
                        TagInfo info;
                        if (t.Type == Token.Comment)
                        {
                            // Handle comment tags
                            tagName = t.Text;
                            info = new TagInfo(t.Text, commentTag);
                        }
                        else if (t.Type == Token.InlineServerScript)
                        {
                            // Handle server-side script tags
                            string script = t.Text.Trim();
                            script = script.Substring(1);
                            tagName = script;
                            if (script.StartsWith("%@"))
                            {
                                // Directives are block tags
                                info = new TagInfo(script, directiveTag);
                            }
                            else
                            {
                                // Other server side script tags aren't
                                info = new TagInfo(script, otherServerSideScriptTag);
                            }
                        }
                        else
                        {
                            // Otherwise, this is a normal tag, and try to get a TagInfo for it
                            tagName = t.Text;
                            if (!tagTable.TryGetValue(tagName, out info) || info == null)
                            {
                                // if we couldn't find one, create a copy of the unknownTag with a new tagname
                                if (tagName.IndexOf(':') > -1)
                                {
                                    // If it is a prefixed tag, it's probably unknown XML
                                    info = new TagInfo(tagName, unknownXmlTag);
                                }
                                else if (writer is XmlWriter)
                                {
                                    info = new TagInfo(tagName, nestedXmlTag);
                                }
                                else
                                {
                                    // If it is a not prefixed, it's probably an unknown HTML tag
                                    info = new TagInfo(tagName, unknownHtmlTag);
                                }
                            }
                            else
                            {
                                // If it's not an unknown tag, converting to the desired case (and leave as is for PreserveCase)
                                if ((options.ElementCasing == HtmlFormatterCase.LowerCase) || makeXhtml)
                                {
                                    tagName = info.TagName;
                                }
                                else if (options.ElementCasing == HtmlFormatterCase.UpperCase)
                                {
                                    tagName = info.TagName.ToUpper();
                                }
                            }
                        }

                        if (previousTag == null)
                        {
                            // Special case for the first tag seen
                            previousTag = new FormatInfo(info, false);
                            // Since this is the first tag, set it's indent to 0
                            previousTag.indent = 0;

                            // Push it on the stack
                            tagStack.Push(previousTag);
                            // And output the preceeding text
                            writer.Write(text);

                            if (info.IsXml)
                            {
                                // When we encounter an xml block, create a new writer to contain the inner content of the xml
                                HtmlWriter newWriter = new XmlWriter(writer.Indent, info.TagName, indentString, maxLineLength);
                                writerStack.Push(newWriter);
                                writer = newWriter;
                            }

                            if (lastToken.Type == Token.ForwardSlash)
                            {
                                // If this is an end tag, output the proper prefix
                                writer.Write("</");
                            }
                            else
                            {
                                writer.Write('<');
                            }
                            // Write the name
                            writer.Write(tagName);
                            // Indicate that we've written out the last text block
                            text = String.Empty;
                        }
                        else
                        {
                            // Put the new tag in the next spot
                            currentTag = new FormatInfo(info, (lastToken.Type == Token.ForwardSlash));

                            WhiteSpaceType whiteSpaceType;
                            if (previousTag.isEndTag)
                            {
                                // If the previous tag is an end tag, we need to check the following whitespace
                                whiteSpaceType = previousTag.tagInfo.FollowingWhiteSpaceType;
                            }
                            else
                            {
                                // otherwise check the initial inner whitespace
                                whiteSpaceType = previousTag.tagInfo.InnerWhiteSpaceType;
                            }

                            // Flag that indicates if the previous tag (before the text) is an inline tag
                            bool inline = previousTag.tagInfo.IsInline;
                            bool emptyXml = false;
                            bool firstOrLastUnknownXmlText = false;

                            if (writer is XmlWriter)
                            {
                                // if we're in an xml block
                                XmlWriter xmlWriter = (XmlWriter)writer;

                                if (xmlWriter.IsUnknownXml)
                                {
                                    // Special case for unknown XML tags
                                    // Determine if this is the first or last xml text in an unknown xml tag, so we know to preserve the text content here
                                    firstOrLastUnknownXmlText = (((previousTag.isBeginTag) && (previousTag.tagInfo.TagName.ToLower() == xmlWriter.TagName.ToLower())) ||
                                        ((currentTag.isEndTag) && (currentTag.tagInfo.TagName.ToLower() == xmlWriter.TagName.ToLower()))) &&
                                        (!FormattedTextWriter.IsWhiteSpace(text));
                                }

                                if (previousTag.isBeginTag)
                                {
                                    if (FormattedTextWriter.IsWhiteSpace(text))
                                    {
                                        if ((xmlWriter.IsUnknownXml) && (currentTag.isEndTag) &&
                                            (previousTag.tagInfo.TagName.ToLower() == currentTag.tagInfo.TagName.ToLower()))
                                        {
                                            // Special case for unknown XML tags:
                                            // If the previous tag is an open tag and the next tag is the corresponding close tag and the text is only whitespace, also
                                            // treat the tag as inline, so the begin and end tag appear on the same line
                                            inline = true;
                                            emptyXml = true;
                                            // Empty the text since we want the open and close tag to be touching
                                            text = "";
                                        }
                                    }
                                    else
                                    {
                                        if (!xmlWriter.IsUnknownXml)
                                        {
                                            // If there is non-whitespace text and we're in a normal Xml block, then remember that there was text
                                            xmlWriter.ContainsText = true;
                                        }
                                    }
                                }
                            }

                            // Flag that indicates if we want to preserve whitespace in the front of the text
                            bool frontWhitespace = true;

                            if ((previousTag.isBeginTag) && (previousTag.tagInfo.PreserveContent))
                            {
                                // If the previous tag is a begin tag and we're preserving the content as-is, just write out the text
                                writer.Write(text);
                            }
                            else
                            {
                                if (whiteSpaceType == WhiteSpaceType.NotSignificant)
                                {
                                    // If the whitespace is not significant in this location
                                    if (!inline && !firstOrLastUnknownXmlText)
                                    {
                                        // If the previous tag is not an inline tag, write out a new line
                                        writer.WriteLineIfNotOnNewLine();
                                        // Since we've written out a newline, we no longer need to preserve front whitespace
                                        frontWhitespace = false;
                                    }
                                }
                                else if (whiteSpaceType == WhiteSpaceType.Significant)
                                {
                                    // If the whitespace in this location is significant
                                    if (FormattedTextWriter.HasFrontWhiteSpace(text))
                                    {
                                        // If there is whitespace in the front, that means we can insert more whitespace without
                                        // changing rendering behavior
                                        if (!inline && !firstOrLastUnknownXmlText)
                                        {
                                            // Only insert a new line if the tag isn't inline
                                            writer.WriteLineIfNotOnNewLine();
                                            frontWhitespace = false;
                                        }
                                    }
                                }
                                else if (whiteSpaceType == WhiteSpaceType.CarryThrough)
                                {
                                    // If the whitespace in this location is carry through (meaning whitespace at the end of the previous
                                    // text block eats up any whitespace in this location
                                    if ((sawWhiteSpace) || (FormattedTextWriter.HasFrontWhiteSpace(text)))
                                    {
                                        // If the last text block ended in whitspace or if there is already whitespace in this location
                                        // we can add a new line
                                        if (!inline && !firstOrLastUnknownXmlText)
                                        {
                                            // Only add it if the previous tag isn't inline
                                            writer.WriteLineIfNotOnNewLine();
                                            frontWhitespace = false;
                                        }
                                    }
                                }

                                if (previousTag.isBeginTag)
                                {
                                    // If the previous tag is a begin tag
                                    if (!previousTag.tagInfo.NoIndent && !inline)
                                    {
                                        // Indent if desired
                                        writer.Indent++;
                                    }
                                }

                                // Special case for unknown XML tags:
                                if (firstOrLastUnknownXmlText)
                                {
                                    writer.Write(text);
                                }
                                else
                                {
                                    writer.WriteLiteral(text, frontWhitespace);
                                }
                            }

                            if (currentTag.isEndTag)
                            {
                                // If the currentTag is an end tag
                                if (!currentTag.tagInfo.NoEndTag)
                                {
                                    // Figure out where the corresponding begin tag is
                                    List<FormatInfo> popped = new List<FormatInfo>();
                                    FormatInfo formatInfo = null;

                                    bool foundOpenTag = false;

                                    bool allowPartial = false;
                                    if ((currentTag.tagInfo.Flags & FormattingFlags.AllowPartialTags) != 0)
                                    {
                                        // Once we've exited a tag that allows partial tags, clear the flag
                                        allowPartial = true;
                                    }

                                    // Start popping off the tag stack if there are tags on the stack
                                    if (tagStack.Count > 0)
                                    {
                                        // Keep popping until we find the right tag, remember what we've popped off
                                        formatInfo = (FormatInfo)tagStack.Pop();
                                        popped.Add(formatInfo);
                                        while ((tagStack.Count > 0) && (formatInfo.tagInfo.TagName.ToLower() != currentTag.tagInfo.TagName.ToLower()))
                                        {
                                            if ((formatInfo.tagInfo.Flags & FormattingFlags.AllowPartialTags) != 0)
                                            {
                                                // Special case for tags that allow partial tags inside of them.
                                                allowPartial = true;
                                                break;
                                            }
                                            formatInfo = (FormatInfo)tagStack.Pop();
                                            popped.Add(formatInfo);
                                        }

                                        if (formatInfo.tagInfo.TagName.ToLower() != currentTag.tagInfo.TagName.ToLower())
                                        {
                                            // If we didn't find the corresponding open tag, push everything back on
                                            for (int i = popped.Count - 1; i >= 0; i--)
                                            {
                                                tagStack.Push(popped[i]);
                                            }
                                        }
                                        else
                                        {
                                            foundOpenTag = true;
                                            for (int i = 0; i < popped.Count - 1; i++)
                                            {
                                                FormatInfo fInfo = (FormatInfo)popped[i];
                                                if (fInfo.tagInfo.IsXml)
                                                {
                                                    // If we have an xml tag that was unclosed, we need to clean up the xml stack
                                                    if (writerStack.Count > 1)
                                                    {
                                                        HtmlWriter oldWriter = writerStack.Pop();
                                                        writer = writerStack.Peek();
                                                        // Write out the contents of the old writer
                                                        writer.Write(oldWriter.Content);
                                                    }
                                                }

                                                if (!fInfo.tagInfo.NoEndTag)
                                                {
                                                    writer.WriteLineIfNotOnNewLine();
                                                    writer.Indent = fInfo.indent;
                                                    if ((makeXhtml) && (!allowPartial))
                                                    {
                                                        // If we're trying to be XHTML compliant, close unclosed child tags
                                                        // Don't close if we are under a tag that allows partial tags
                                                        writer.Write("</" + fInfo.tagInfo.TagName + ">");
                                                    }
                                                }
                                            }

                                            // Set the indent to the indent of the corresponding open tag
                                            writer.Indent = formatInfo.indent;
                                        }
                                    }
                                    if (foundOpenTag || allowPartial)
                                    {
                                        // Only write out the close tag if there was a corresponding open tag or we are under
                                        // a tag that allows partial tags
                                        if ((!emptyXml) &&
                                            (!firstOrLastUnknownXmlText) &&
                                            (!currentTag.tagInfo.IsInline) &&
                                            (!currentTag.tagInfo.PreserveContent) &&
                                            (FormattedTextWriter.IsWhiteSpace(text) ||
                                            FormattedTextWriter.HasBackWhiteSpace(text) ||
                                            (currentTag.tagInfo.FollowingWhiteSpaceType == WhiteSpaceType.NotSignificant)
                                            ) &&
                                            (!(currentTag.tagInfo is TDTagInfo) ||
                                            FormattedTextWriter.HasBackWhiteSpace(text)
                                            )
                                            )
                                        {
                                            // Insert a newline before the next tag, if allowed
                                            writer.WriteLineIfNotOnNewLine();
                                        }
                                        // Write out the end tag prefix
                                        writer.Write("</");
                                        // Finally, write out the tag name
                                        writer.Write(tagName);
                                    }
                                    else
                                    {
                                        ignoredLastTag = true;
                                    }

                                    if (currentTag.tagInfo.IsXml)
                                    {
                                        // If we have an xml tag that was unclosed, we need to clean up the xml stack
                                        if (writerStack.Count > 1)
                                        {
                                            HtmlWriter oldWriter = writerStack.Pop();
                                            writer = writerStack.Peek();
                                            // Write out the contents of the old writer
                                            writer.Write(oldWriter.Content);
                                        }
                                    }
                                }
                                else
                                {
                                    ignoredLastTag = true;
                                }
                            }
                            else
                            {
                                // If the currentTag is a begin tag
                                bool done = false;
                                // Close implicitClosure tags
                                while (!done && (tagStack.Count > 0))
                                {
                                    // Peek at the top of the stack to see the last unclosed tag
                                    FormatInfo fInfo = (FormatInfo)tagStack.Peek();
                                    // If the currentTag can't be a child of that tag, then we need to close that tag
                                    done = fInfo.tagInfo.CanContainTag(currentTag.tagInfo);
                                    if (!done)
                                    {
                                        // Pop it off and write a close tag for it
                                        // REVIEW: Will XML tags always be able to contained in any tag?  If not we should be cleaning up the writerStack as well...
                                        tagStack.Pop();
                                        writer.Indent = fInfo.indent;
                                        // If we're trying to be XHTML compliant, write in the end tags
                                        if (makeXhtml)
                                        {
                                            if (!fInfo.tagInfo.IsInline)
                                            {
                                                // Only insert a newline if we are allowed to
                                                writer.WriteLineIfNotOnNewLine();
                                            }
                                            writer.Write("</" + fInfo.tagInfo.TagName + ">");
                                        }
                                    }
                                }

                                // Remember the indent so we can properly indent the corresponding close tag for this open tag
                                currentTag.indent = writer.Indent;

                                if ((!firstOrLastUnknownXmlText) &&
                                    (!currentTag.tagInfo.IsInline) &&
                                    (!currentTag.tagInfo.PreserveContent) &&
                                    ((FormattedTextWriter.IsWhiteSpace(text) || FormattedTextWriter.HasBackWhiteSpace(text)) ||
                                    ((text.Length == 0) &&
                                    (((previousTag.isBeginTag) && (previousTag.tagInfo.InnerWhiteSpaceType == WhiteSpaceType.NotSignificant)) ||
                                    ((previousTag.isEndTag) && (previousTag.tagInfo.FollowingWhiteSpaceType == WhiteSpaceType.NotSignificant))
                                    )
                                    )
                                    )
                                    )
                                {
                                    // Insert a newline before the currentTag if we are allowed to
                                    writer.WriteLineIfNotOnNewLine();
                                }

                                if (!currentTag.tagInfo.NoEndTag)
                                {
                                    // Only push tags with close tags onto the stack
                                    tagStack.Push(currentTag);
                                }
                                else
                                {
                                    // If this tag doesn't have a close tag, remember that it is self terminating
                                    sawSelfTerminatingTag = true;
                                }

                                if (currentTag.tagInfo.IsXml)
                                {
                                    // When we encounter an xml block, create a new writer to contain the inner content of the xml
                                    HtmlWriter newWriter = new XmlWriter(writer.Indent, currentTag.tagInfo.TagName, indentString, maxLineLength);
                                    writerStack.Push(newWriter);
                                    writer = newWriter;
                                }

                                writer.Write('<');
                                // Finally, write out the tag name
                                writer.Write(tagName);
                            }

                            // Remember if the text ended in whitespace
                            sawWhiteSpace = FormattedTextWriter.HasBackWhiteSpace(text);

                            // Clear out the text, since we have already outputted it
                            text = String.Empty;

                            previousTag = currentTag;
                        }
                        break;
                    case Token.ServerScriptBlock:
                    case Token.ClientScriptBlock:
                    case Token.Style:
                    case Token.TextToken:
                        // Remember all these types of tokens as text so we can output them between the tags
                        if (makeXhtml)
                        {
                            // UNDONE: Need to implement this in the tokenizer, etc... 
                            text += t.Text.Replace("&nbsp;", "&#160;");
                        }
                        else
                        {
                            text += t.Text;
                        }
                        break;
                    case Token.Whitespace:
                        if (t.Text.Length > 0)
                        {
                            writer.Write(' ');
                        }
                        break;
                    default:
                        Debug.Fail("Invalid token type!");
                        break;
                }
                // Remember what the last token was
                lastToken = t;

                // Get the next token
                t = HtmlTokenizer.GetNextToken(t);
            }

            if (text.Length > 0)
            {
                // Write out the last text if there is any
                writer.Write(text);
            }

            while (writerStack.Count > 1)
            {
                // If we haven't cleared out the writer stack, do it
                HtmlWriter oldWriter = writerStack.Pop();
                writer = writerStack.Peek();
                writer.Write(oldWriter.Content);
            }

            // Flush the writer original
            writer.Flush();
        }

        private sealed class FormatInfo
        {
            public readonly TagInfo tagInfo;
            public bool isEndTag;
            public int indent;

            public FormatInfo(TagInfo info, bool isEnd)
            {
                tagInfo = info;
                isEndTag = isEnd;
            }

            public bool isBeginTag
            {
                get
                {
                    return !isEndTag;
                }
            }
        }
    }
}
