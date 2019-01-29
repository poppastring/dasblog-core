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

namespace newtelligence.DasBlog.Runtime.Util.Html 
{
	public sealed class Token 
	{
		public const int Whitespace = 0;
		public const int TagName = 1;
		public const int AttrName = 2;
		public const int AttrVal = 3;
		public const int TextToken = 4;
		public const int SelfTerminating = 5;
		public const int Empty = 6;
		public const int Comment = 7;
		public const int Error = 8;

		public const int OpenBracket = 10;
		public const int CloseBracket = 11;
		public const int ForwardSlash = 12;
		public const int DoubleQuote = 13;
		public const int SingleQuote = 14;
		public const int EqualsChar = 15;

		public const int ClientScriptBlock = 20;
		public const int Style = 21;
		public const int InlineServerScript = 22;
		public const int ServerScriptBlock = 23;
		public const int XmlDirective = 24;

		public Token(int type, int endState, int startIndex, int endIndex, char[] chars, int charsLength) 
		{
			Type = type;
			Chars = chars;
			CharsLength = charsLength;
			StartIndex = startIndex;
			EndIndex = endIndex;
			EndState = endState;
		}

		internal char[] Chars 
		{
            get;
            private set;
		}

		internal int CharsLength 
		{
            get;
            private set;
		}

		public int EndIndex 
		{
            get;
            private set;
		}

		public int EndState 
		{
            get;
            private set;
		}

		public int Length 
		{
            get
            {
                return EndIndex - StartIndex;
            }
		}

		public int StartIndex 
		{
            get;
            private set;
		}

        private string text;

		public string Text 
		{
			get 
			{
                if (text == null)
                {
                    text = new string(Chars, StartIndex, Length);
                }

                return text;
			}
		}

		public int Type 
		{
            get;
            private set;
		}

#if DEBUG
		public override string ToString() 
		{
			string s = "\'" + Text + "\'";
			switch (Type) 
			{
				case Whitespace:
					s += "(Whitespace)";
					break;
				case ForwardSlash:
					s += "(ForwardSlash)";
					break;
				case DoubleQuote:
					s += "(DoubleQuote)";
					break;
				case SingleQuote:
					s += "(SingleQuote)";
					break;
				case OpenBracket:
					s += "(OpenBracket)";
					break;
				case CloseBracket:
					s += "(CloseBracket)";
					break;
				case EqualsChar:
					s += "(Equals)";
					break;
				case TagName:
					s += "(Tag)";
					break;
				case AttrName:
					s += "(AttrName)";
					break;
				case AttrVal:
					s += "(AttrVal)";
					break;
				case TextToken:
					s += "(Text)";
					break;
				case SelfTerminating:
					s += "(SelfTerm)";
					break;
				case Empty:
					s += "(Empty)";
					break;
				case Comment:
					s += "(Comment)";
					break;
				case Error:
					s += "(Error)";
					break;
				case ClientScriptBlock:
					s += "(ClientScriptBlock)";
					break;
				case Style:
					s += "(Style)";
					break;
				case InlineServerScript:
					s += "(InlineServerScript)";
					break;
				case ServerScriptBlock:
					s += "(ServerScriptBlock)";
					break;
			}

			return s;
		}
#endif //DEBUG
	}
}
