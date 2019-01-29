using System.IO;

namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
    internal class HtmlWriter 
    {
        private FormattedTextWriter _writer;
        private TextWriter _baseWriter;
        private int _maxLineLength;

        public HtmlWriter(TextWriter writer, string indentString, int maxLineLength) 
        {
            _baseWriter = writer;
            _maxLineLength = maxLineLength;
            _writer = new FormattedTextWriter(_baseWriter, indentString);
        }

        protected TextWriter BaseWriter 
        {
            get 
            {
                return _baseWriter;
            }
        }

        public virtual string Content 
        {
            get 
            {
                _writer.Flush();
                return _baseWriter.ToString();
            }
        }

        public int Indent 
        {
            get 
            {
                return _writer.Indent;
            }
            set 
            {
                _writer.Indent = value;
            }
        }

        public void Flush() 
        {
            _writer.Flush();
        }

        public FormattedTextWriter Writer 
        {
            get 
            {
                return _writer;
            }
        }

        public virtual void Write(char c) 
        {
            _writer.Write(c);
        }

        public virtual void Write(string s) 
        {
            _writer.Write(s);
        }

        public virtual void WriteLiteral(string s, bool frontWhiteSpace) 
        {
            _writer.WriteLiteralWrapped(FormattedTextWriter.Trim(s, frontWhiteSpace), _maxLineLength);
        }

        public virtual void WriteLineIfNotOnNewLine() 
        {
            _writer.WriteLineIfNotOnNewLine();
        }
    }
}
