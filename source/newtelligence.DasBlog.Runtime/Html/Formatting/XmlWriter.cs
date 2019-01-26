using System.IO;
using System.Text;

namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class XmlWriter : HtmlWriter 
	{
		private bool _containsText;
		private StringBuilder _unformatted;
		private string _tagName;
		private bool _isUnknownXml;

		public XmlWriter(int initialIndent, string tagName, string indentString, int maxLineLength) : base(new StringWriter(), indentString, maxLineLength) 
		{
			Writer.Indent = initialIndent;
			_unformatted = new StringBuilder();
			_tagName = tagName;
			_isUnknownXml = (_tagName.IndexOf(':') > -1);
		}

		public bool ContainsText 
		{
			get 
			{
				return _containsText;
			}
			set 
			{
				_containsText = value;
			}
		}

		public override string Content 
		{
			get 
			{
				if (ContainsText) 
				{
					return _unformatted.ToString();
				}
				else 
				{
					return base.Content;                    
				}
			}
		}

		public string TagName 
		{
			get 
			{
				return _tagName;
			}
		}

		public bool IsUnknownXml 
		{
			get 
			{
				return _isUnknownXml;
			}
		}

		public override void Write(char c) 
		{
			base.Write(c);
			// Keep an unformatted copy around
			_unformatted.Append(c);
		}

		public override void Write(string s) 
		{
			base.Write(s);
			// Keep an unformatted copy around
			_unformatted.Append(s);
		}

		public override void WriteLiteral(string s, bool frontWhiteSpace) 
		{
			base.WriteLiteral(s, frontWhiteSpace);
			// Keep an unformatted copy around
			_unformatted.Append(s);
		}
	}
}