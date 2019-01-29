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

using System;
using System.IO;
using System.Text;

namespace newtelligence.DasBlog.Runtime.Util.Html 
{
    public static class HtmlTokenizer 
    {
        public static Token GetFirstToken(char[] chars) 
        {
            if (chars == null) 
            {
                throw new ArgumentNullException("chars");
            }

            return GetNextToken(chars, chars.Length, 0, 0);
        }

        public static Token GetFirstToken(char[] chars, int length, int initialState) 
        {
            return GetNextToken(chars, length, 0, initialState);
        }

        public static Token GetNextToken(Token token) 
        {
            if (token == null) 
            {
                throw new ArgumentNullException("token");
            }
            return GetNextToken(token.Chars, token.CharsLength, token.EndIndex, token.EndState);
        }

        public static Token GetNextToken(char[] chars, int length, int startIndex, int startState) 
        {
            if (chars == null) 
            {
                throw new ArgumentNullException("chars");
            }

            if (startIndex >= length) 
            {
                return null;
            }

            int state = startState;

            bool inScript = ((startState & HtmlTokenizerStates.ScriptState) != 0);
            int scriptState = (inScript ? HtmlTokenizerStates.ScriptState : 0);

            bool inStyle = ((startState & HtmlTokenizerStates.StyleState) != 0);
            int styleState = (inStyle ? HtmlTokenizerStates.StyleState : 0);

            bool hasRunAt = ((startState & HtmlTokenizerStates.RunAtState) != 0);
            int runAtState = (hasRunAt ? HtmlTokenizerStates.RunAtState : 0);

            bool hasRunAtServer = ((startState & HtmlTokenizerStates.RunAtServerState) != 0);
            int runAtServerState = (hasRunAtServer ? HtmlTokenizerStates.RunAtServerState : 0);

            int index = startIndex;
            int tokenStart = startIndex; // inclusive
            int tokenEnd = startIndex; // exclusive
            Token token = null;

            while ((token == null) && (index < length)) 
            {
                char c = chars[index];
                switch (state & 0xFF) 
                {
                    case HtmlTokenizerStates.Text:
                        if (c == '<') 
                        {
                            state = HtmlTokenizerStates.StartTag;
                            tokenEnd = index;
                            token = new Token(Token.TextToken, state, tokenStart, tokenEnd, chars, length);
                        }
                        break;
                    case HtmlTokenizerStates.StartTag:
                        if (c == '<') 
                        {
                            if ((index + 1 < length) && (chars[index + 1] == '%')) 
                            {
                                // Include the open bracket in a server-side script token
                                state = HtmlTokenizerStates.ServerSideScript | scriptState | styleState;
                                tokenStart = index;
                            }
                            else 
                            {
                                state = HtmlTokenizerStates.ExpTag | scriptState | styleState;
                                tokenEnd = index + 1;
                                token = new Token(Token.OpenBracket, state, tokenStart, tokenEnd, chars, length);
                            }
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.ExpTag:
                        if (c == '/') 
                        {
                            state = HtmlTokenizerStates.ForwardSlash | scriptState | styleState;
                            tokenEnd = index;
                            token = new Token(Token.Empty, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '!') 
                        {
                            state = HtmlTokenizerStates.BeginCommentTag1 | scriptState | styleState;
                            tokenStart = index;
                        }
                        else if (c == '%') 
                        {
                            state = HtmlTokenizerStates.ServerSideScript;
                            tokenStart = index;
                        }
                        else if (IsWordChar(c)) 
                        {
                            // If we get a word char, go to the in tag state
                            state = HtmlTokenizerStates.InTagName | scriptState | styleState;
                            tokenStart = index;
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.ServerSideScript:
                        int endServerSideScriptIndex = IndexOf(chars, index, length, "%>");
                        if (endServerSideScriptIndex > -1) 
                        {
                            state = HtmlTokenizerStates.Text;
                            // Include the percent and close bracket in the server side script
                            tokenEnd = endServerSideScriptIndex + 2;
                            token = new Token(Token.InlineServerScript, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            index = length;
                            tokenEnd = index;
                        }
                        break;
                    case HtmlTokenizerStates.ForwardSlash:
                        if (c == '/') 
                        {
                            state = HtmlTokenizerStates.ExpTagAfterSlash | scriptState | styleState;
                            tokenEnd = index + 1;
                            token = new Token(Token.ForwardSlash, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.ExpTagAfterSlash:
                        if (IsWordChar(c)) 
                        {
                            // If we get a word char, go to the in tag state
                            state = HtmlTokenizerStates.InTagName | scriptState | styleState;
                            tokenStart = index;
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InTagName:
                        if (IsWhitespace(c)) 
                        {
                            // If we hit whitespace, return a token
                            state = HtmlTokenizerStates.ExpAttr;
                            tokenEnd = index;
                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
                            if (tagName.ToLower().Equals("script")) 
                            {
                                if (!inScript) 
                                {
                                    state |= HtmlTokenizerStates.ScriptState;
                                }
                            }
                            else if (tagName.ToLower().Equals("style")) 
                            {
                                if (!inStyle) 
                                {
                                    state |= HtmlTokenizerStates.StyleState;
                                }
                            }
                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag;
                            tokenEnd = index;
                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
                            if (tagName.ToLower().Equals("script")) 
                            {
                                if (!inScript) 
                                {
                                    state |= HtmlTokenizerStates.ScriptState;
                                }
                            }
                            else if (tagName.ToLower().Equals("style")) 
                            {
                                if (!inStyle) 
                                {
                                    state |= HtmlTokenizerStates.StyleState;
                                }
                            }
                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWordChar(c)) 
                        {
                            // Keep traversing if we get a word char
                        }
                        else if (c == '/') 
                        {
                            state = HtmlTokenizerStates.SelfTerminating;
                            tokenEnd = index;
                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
                            if (tagName.ToLower().Equals("script")) 
                            {
                                if (!inScript) 
                                {
                                    state |= HtmlTokenizerStates.ScriptState;
                                }
                            }
                            else if (tagName.ToLower().Equals("style")) 
                            {
                                if (!inStyle) 
                                {
                                    state |= HtmlTokenizerStates.StyleState;
                                }
                            }
                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.BeginCommentTag1:
                        if (c == '-') 
                        {
                            state = HtmlTokenizerStates.BeginCommentTag2;
                        }
                        else if (IsWordChar(c)) 
                        {
                            // This will allow the tokenizer to recognize xml directives as normal tags
                            state = HtmlTokenizerStates.XmlDirective;
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.BeginCommentTag2:
                        if (c == '-') 
                        {
                            state = HtmlTokenizerStates.InCommentTag;
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InCommentTag:
                        if (c == '-') 
                        {
                            state = HtmlTokenizerStates.EndCommentTag1;
                        }
                        break;
                    case HtmlTokenizerStates.EndCommentTag1:
                        if (c == '-') 
                        {
                            state = HtmlTokenizerStates.EndCommentTag2;
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.InCommentTag;
                        }
                        break;
                    case HtmlTokenizerStates.EndCommentTag2:
                        if (Char.IsWhiteSpace(c)) 
                        {
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag;
                            tokenEnd = index;
                            token = new Token(Token.Comment, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.InCommentTag;
                        }
                        break;
                    case HtmlTokenizerStates.XmlDirective:
                        if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag;
                            tokenEnd = index;
                            token = new Token(Token.XmlDirective, state, tokenStart, tokenEnd, chars, length);
                        }
                        break;
                    case HtmlTokenizerStates.ExpAttr:
                        if (IsWordChar(c)) 
                        {
                            state = HtmlTokenizerStates.InAttr | scriptState | styleState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '/') 
                        {
                            state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWhitespace(c)) 
                        {
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InAttr:
                        if (IsWhitespace(c)) 
                        {
                            // If we hit whitespace, return a token
                            state = HtmlTokenizerStates.ExpEquals | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if (inScript) 
                            {
                                // Check if this is a runat="server" script block
                                if (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "runat") 
                                {
                                    state |= HtmlTokenizerStates.RunAtState;
                                }
                            }

                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '=') 
                        {
                            state = HtmlTokenizerStates.ExpEquals | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if (inScript) 
                            {
                                // Check if this is a runat="server" script block
                                if (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "runat") 
                                {
                                    state |= HtmlTokenizerStates.RunAtState;
                                }
                            }

                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '/') 
                        {
                            state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState;
                            tokenEnd = index;
                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
                        }                        
                        else if (IsWordChar(c)) 
                        {
                            // Keep traversing if we get a word char
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }

                        break;
                    case HtmlTokenizerStates.ExpEquals:
                        if (c == '=') 
                        {
                            state = HtmlTokenizerStates.ExpAttrVal | scriptState | styleState | runAtState | runAtServerState;
                            tokenStart = index;
                            tokenEnd = index + 1;
                            token = new Token(Token.EqualsChar, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '/') 
                        {
                            state = HtmlTokenizerStates.SelfTerminating;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWordChar(c)) 
                        {
                            state = HtmlTokenizerStates.InAttr | scriptState | styleState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWhitespace(c)) 
                        {
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }

                        break;
                    case HtmlTokenizerStates.EqualsChar:
                        if (c == '=') 
                        {
                            state = HtmlTokenizerStates.ExpAttrVal | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index + 1;
                            token = new Token(Token.EqualsChar, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.ExpAttrVal:
                        if (c == '\'') 
                        {
                            state = HtmlTokenizerStates.BeginSingleQuote | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '\"') 
                        {
                            state = HtmlTokenizerStates.BeginDoubleQuote | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWordChar(c)) 
                        {
                            state = HtmlTokenizerStates.InAttrVal | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index;
                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (IsWhitespace(c)) 
                        {
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.BeginDoubleQuote:
                        if (c == '\"') 
                        {
                            state = HtmlTokenizerStates.InDoubleQuoteAttrVal | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index + 1;
                            token = new Token(Token.DoubleQuote, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InDoubleQuoteAttrVal:
                        if (c == '\"') 
                        {
                            state = HtmlTokenizerStates.EndDoubleQuote | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
                            {
                                state |= HtmlTokenizerStates.RunAtServerState;
                            }

                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
                        }
                        break;
                    case HtmlTokenizerStates.EndDoubleQuote:
                        if (c == '\"') 
                        {
                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
                            tokenEnd = index + 1;
                            token = new Token(Token.DoubleQuote, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.BeginSingleQuote:
                        if (c == '\'') 
                        {
                            state = HtmlTokenizerStates.InSingleQuoteAttrVal | scriptState | styleState | runAtState | runAtServerState;
                            tokenEnd = index + 1;
                            token = new Token(Token.SingleQuote, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InSingleQuoteAttrVal:
                        if (c == '\'') 
                        {
                            state = HtmlTokenizerStates.EndSingleQuote | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
                            {
                                state |= HtmlTokenizerStates.RunAtServerState;
                            }

                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
                        }
                        break;
                    case HtmlTokenizerStates.EndSingleQuote:
                        if (c == '\'') 
                        {
                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
                            tokenEnd = index + 1;
                            token = new Token(Token.SingleQuote, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.InAttrVal:
                        if (IsWhitespace(c)) 
                        {
                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
                            {
                                state |= HtmlTokenizerStates.RunAtServerState;
                            }

                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
                            tokenEnd = index;

                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
                            {
                                state |= HtmlTokenizerStates.RunAtServerState;
                            }

                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
                        }
                        else if (c == '/') 
                        {
                            // This check fixes a bug when there's a forward slash in an attrval (since Trident likes to remove
                            // double quotes from our attrvals
                            if (((index + 1) < length) && (chars[index + 1] == '>')) 
                            {
                                state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState | runAtServerState;
                                tokenEnd = index;

                                if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
                                {
                                    state |= HtmlTokenizerStates.RunAtServerState;
                                }

                                token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
                            }
                        }
                        break;
                    case HtmlTokenizerStates.SelfTerminating:
                        if ((c == '/') && (index + 1 < length) && (chars[index + 1] == '>')) 
                        {
                            state = HtmlTokenizerStates.Text;
                            tokenEnd = index + 2;
                            token = new Token(Token.SelfTerminating, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.EndTag:
                        if (c == '>') 
                        {
                            if (inScript) 
                            {
                                state = HtmlTokenizerStates.Script | scriptState | styleState | runAtServerState;
                            }
                            else if (inStyle) 
                            {
                                state = HtmlTokenizerStates.Style | scriptState | styleState;
                            }
                            else 
                            {
                                state = HtmlTokenizerStates.Text;
                            }
                            tokenEnd = index + 1;
                            token = new Token(Token.CloseBracket, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            state = HtmlTokenizerStates.Error;
                        }
                        break;
                    case HtmlTokenizerStates.Script:
                        int endScriptIndex = IndexOf(chars, index, length, "</script>");
                        if (endScriptIndex > -1) 
                        {
                            state = HtmlTokenizerStates.StartTag | scriptState | styleState | runAtServerState;
                            tokenEnd = endScriptIndex;
                            if (hasRunAtServer) 
                            {
                                token = new Token(Token.ServerScriptBlock, state, tokenStart, tokenEnd, chars, length);
                            }
                            else 
                            {
                                token = new Token(Token.ClientScriptBlock, state, tokenStart, tokenEnd, chars, length);
                            }
                        }
                        else 
                        {
                            index = length - 1;
                            tokenEnd = index;
                        }
                        break;
                    case HtmlTokenizerStates.Style:
                        int endStyleIndex = IndexOf(chars, index, length, "</style>");
                        if (endStyleIndex > -1) 
                        {
                            state = HtmlTokenizerStates.StartTag | scriptState | styleState;
                            tokenEnd = endStyleIndex;
                            token = new Token(Token.Style, state, tokenStart, tokenEnd, chars, length);
                        }
                        else 
                        {
                            index = length - 1;
                            tokenEnd = index;
                        }
                        break;
                    case HtmlTokenizerStates.Error:
                        if (c == '>') 
                        {
                            state = HtmlTokenizerStates.EndTag;
                            tokenEnd = index;
                            token = new Token(Token.Error, state, tokenStart, tokenEnd, chars, length);
                        }
                        break;
                }
                
                index++;
            }

            if ((index >= length) && (token == null)) 
            {
                int tokenType;
                // Some tokens can span multiple lines, so return a token if we haven't found one yet
                switch (state & 0xFF) 
                {
                    case HtmlTokenizerStates.Text:
                        tokenType = Token.TextToken;
                        break;
                    case HtmlTokenizerStates.Script:
                        if (hasRunAtServer) 
                        {
                            tokenType = Token.ServerScriptBlock;
                        }
                        else 
                        {
                            tokenType = Token.ClientScriptBlock;
                        }
                        break;
                    case HtmlTokenizerStates.Style:
                        tokenType = Token.Style;
                        break;
                    case HtmlTokenizerStates.ServerSideScript:
                        tokenType = Token.InlineServerScript;
                        break;
                    case HtmlTokenizerStates.BeginCommentTag1:
                    case HtmlTokenizerStates.BeginCommentTag2:
                    case HtmlTokenizerStates.InCommentTag:
                    case HtmlTokenizerStates.EndCommentTag1:
                    case HtmlTokenizerStates.EndCommentTag2:
                        tokenType = Token.Comment;
                        break;
                    default:
                        tokenType = Token.Error;
                        state = HtmlTokenizerStates.Error;
                        break;
                }
                tokenEnd = index;
                token = new Token(tokenType, state, tokenStart, tokenEnd, chars, length);
            }
            return token;
        }

        private static bool IsWhitespace(char c) 
        {
            return Char.IsWhiteSpace(c);
        }

        private static bool IsWordChar(char c) 
        {
            return (Char.IsLetterOrDigit(c) || (c == '_') || (c == ':') || (c == '#') || (c == '-') || (c == '.'));
        }

        private static int IndexOf(char[] chars, int startIndex, int endColumnNumber, string s) 
        {
            int stringLength = s.Length;
            int end = endColumnNumber - stringLength + 1;
            for (int i = startIndex; i < end; i++) 
            {
                bool success = true;
                for (int j = 0; j < stringLength; j++) 
                {
                    if (Char.ToUpper(chars[i + j]) != Char.ToUpper(s[j])) 
                    {
                        success = false;
                        break;
                    }
                }
                if (success) 
                {
                    return i;
                }
            }
            return -1;
        }

        private static class HtmlTokenizerStates 
        {
            public const int Text = 0;
            public const int StartTag = 1;
            public const int ExpTag = 2;
            public const int ForwardSlash = 3;
            public const int ExpTagAfterSlash = 4;
            public const int InTagName = 5;
            public const int ExpAttr = 6;
            public const int InAttr = 7;
            public const int ExpEquals = 8;
            public const int ExpAttrVal = 9;
            public const int InDoubleQuoteAttrVal = 10;
            public const int EndDoubleQuote = 11;
            public const int InSingleQuoteAttrVal = 12;
            public const int EndSingleQuote = 13;
            public const int InAttrVal = 14;
            public const int SelfTerminating = 15;
            public const int Error = 16;
            public const int EndTag = 17;

            public const int EqualsChar = 18;
            public const int BeginDoubleQuote = 19;
            public const int BeginSingleQuote = 20;

            public const int ServerSideScript = 30;

            public const int Script = 40;

            public const int Style = 50;

            public const int XmlDirective = 60;

            public const int BeginCommentTag1 = 100;
            public const int BeginCommentTag2 = 101;
            public const int InCommentTag = 102;
            public const int EndCommentTag1 = 103;
            public const int EndCommentTag2 = 104;

            public const int ScriptState = 0x0100;
            public const int StyleState = 0x0200;
            public const int RunAtState = 0x0400;
            public const int RunAtServerState = 0x800;
        }

#if DEBUG
        public static void Main(string[] args) 
        {
            if (args.Length != 2) 
            {
                Console.WriteLine("Tokenizes an HTML document");
                Console.WriteLine("Usage: HtmlTokenizer <html file> <out file>");
                return;
            }

            MemoryStream memStream = new MemoryStream();
            FileStream fileStream = new FileStream(args[0], FileMode.Open);
            byte[] buffer = new byte[1024];
            int count = 1;
            while (count > 0) 
            {
                count = fileStream.Read(buffer, 0, 1024);
                memStream.Write(buffer, 0, count);
            }
            char[] chars = Encoding.UTF8.GetChars(memStream.ToArray());

            Token t = HtmlTokenizer.GetFirstToken(chars);
            Console.WriteLine(t);
            FileStream stream = new FileStream(args[1], FileMode.Create);
            while (t != null) 
            {
                t = HtmlTokenizer.GetNextToken(t);
                if (t != null) 
                {
                    Console.WriteLine(t);
                    buffer = Encoding.UTF8.GetBytes(t.Text);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            stream.Flush();
            stream.Close();
        }
#endif // DEBUG
    }
}
