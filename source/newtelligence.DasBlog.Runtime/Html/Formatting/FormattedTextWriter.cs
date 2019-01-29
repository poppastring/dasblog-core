using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
    internal sealed class FormattedTextWriter : TextWriter 
    {
        private TextWriter baseWriter;
        private string indentString;
        private int currentColumn;
        private int indentLevel;
        private bool indentPending;
        private bool onNewLine;

        public FormattedTextWriter(TextWriter writer, string indentString) 
        {
            this.baseWriter = writer;
            this.indentString = indentString;
            this.onNewLine = true;
            currentColumn = 0;
        }

        public override Encoding Encoding 
        {
            get 
            {
                return baseWriter.Encoding;
            }
        }

        public override string NewLine 
        {
            get 
            {
                return baseWriter.NewLine;
            }
            set 
            {
                baseWriter.NewLine = value;
            }
        }

        public int Indent 
        {
            get 
            {
                return indentLevel;
            }
            set 
            {
                if (value < 0) 
                {
                    value = 0;
                }
                indentLevel = value;
                Debug.Assert(value >= 0, "Invalid IndentLevel");
            }
        }

        public override void Close() 
        {
            baseWriter.Close();
        }

        public override void Flush() 
        {
            baseWriter.Flush();
        }

        public static bool HasBackWhiteSpace(string s) 
        {
            if ((s == null) || (s.Length == 0)) 
            {
                return false;
            }
            return (Char.IsWhiteSpace(s[s.Length - 1]));
        }

        public static bool HasFrontWhiteSpace(string s) 
        {
            if ((s == null) || (s.Length == 0)) 
            {
                return false;
            }
            return (Char.IsWhiteSpace(s[0]));
        }

        public static bool IsWhiteSpace(string s) 
        {
            for (int i = 0; i < s.Length; i++) 
            {
                if (!Char.IsWhiteSpace(s[i])) 
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the string into a single line seperated by single spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// TODO: Rename to CollapseWhitespace
        private string MakeSingleLine(string s) 
        {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            while (i < s.Length) 
            {
                char c = s[i];
                if (Char.IsWhiteSpace(c)) 
                {
                    builder.Append(' ');
                    while ((i < s.Length) && (Char.IsWhiteSpace(s[i]))) 
                    {
                        i++;
                    }
                }
                else 
                {
                    builder.Append(c);
                    i++;
                }
            }
            return builder.ToString();
        }

        public static string Trim(string text, bool frontWhiteSpace) 
        {
            if (text.Length == 0) 
            {
                // If there is no text, return the empty string
                return String.Empty;
            }

            if (IsWhiteSpace(text)) 
            {
                // If the text is all whitespace
                if (frontWhiteSpace) 
                {
                    // If the caller wanted to preserve front whitespace, then return just one space
                    return " ";
                }
                else 
                {
                    // If the caller isn't trying to preserve anything, return the empty string
                    return String.Empty;
                }
            }

            // Trim off all whitespace
            string t = text.Trim();
            if (frontWhiteSpace && HasFrontWhiteSpace(text)) 
            {
                // Add front whitespace if there was some and we're trying to preserve it
                t = ' ' + t;
            }
            if (HasBackWhiteSpace(text)) 
            {
                // Add back whitespace if there was some
                t = t + ' ';
            }
            return t;
        }

        private void OutputIndent() 
        {
            if (indentPending) 
            {
                for (int i = 0; i < indentLevel; i++) 
                {
                    baseWriter.Write(indentString);
                }
                indentPending = false;
            }
        }

        public void WriteLiteral(string s) 
        {
            if (s.Length != 0) 
            {
                StringReader reader = new StringReader(s);
                // We want to output the first line of the string trimming the back whitespace
                // the middle lines trimming all whitespace
                // and the last line trimming the leading whitespace (which requires a one string lookahead)
                string lastString = reader.ReadLine();
                string nextString = reader.ReadLine();
                while (lastString != null) 
                {
                    Write(lastString);
                    lastString = nextString;
                    nextString = reader.ReadLine();

                    if (lastString != null) 
                    {
                        WriteLine();
                    }
                    if (nextString != null) 
                    {
                        lastString = lastString.Trim();
                    }
                    else if (lastString != null) 
                    {                            
                        lastString = Trim(lastString, false);
                    }
                }
            }
        }

        public void WriteLiteralWrapped(string s, int maxLength) 
        {
            if (s.Length != 0) 
            {
                // First make the string a single line space-delimited string and split on the strings
                string[] tokens = MakeSingleLine(s).Split(null);
                // Preserve the initial whitespace
                if (HasFrontWhiteSpace(s)) 
                {
                    Write(' ');
                }

                // Write out all tokens, wrapping when the length exceeds the specified length
                for (int i = 0; i < tokens.Length; i++) 
                {
                    if (tokens[i].Length > 0) 
                    {
                        Write(tokens[i]);
                        if ((i < (tokens.Length - 1)) && (tokens[i + 1].Length > 0)) 
                        {
                            if (currentColumn > maxLength) 
                            {
                                WriteLine();
                            }
                            else 
                            {
                                Write(' ');
                            }
                        }
                    }
                }

                if (HasBackWhiteSpace(s) && !IsWhiteSpace(s)) 
                {
                    Write(' ');
                }
            }
        }

        public void WriteLineIfNotOnNewLine() 
        {
            if (onNewLine == false) 
            {
                baseWriter.WriteLine();
                onNewLine = true;
                currentColumn = 0;
                indentPending = true;
            }
        }
                
        public override void Write(string s) 
        {
            OutputIndent();
            baseWriter.Write(s);
            onNewLine = false;
            currentColumn += s.Length;
        }

        public override void Write(bool value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(char value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn ++;
        }

        public override void Write(char[] buffer) 
        {
            OutputIndent();
            baseWriter.Write(buffer);
            onNewLine = false;
            currentColumn += buffer.Length;
        }

        public override void Write(char[] buffer, int index, int count) 
        {
            OutputIndent();
            baseWriter.Write(buffer, index, count);
            onNewLine = false;
            currentColumn += count;
        }

        public override void Write(double value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(float value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(int value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(long value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(object value) 
        {
            OutputIndent();
            baseWriter.Write(value);
            onNewLine = false;
            currentColumn += value.ToString().Length;
        }

        public override void Write(string format, object arg0) 
        {
            OutputIndent();
            string s = String.Format(format, arg0);
            baseWriter.Write(s);
            onNewLine = false;
            currentColumn += s.Length;
        }

        public override void Write(string format, object arg0, object arg1) 
        {
            OutputIndent();
            string s = String.Format(format, arg0, arg1);
            baseWriter.Write(s);
            onNewLine = false;
            currentColumn += s.Length;
        }

        public override void Write(string format, params object[] arg) 
        {
            OutputIndent();
            string s = String.Format(format, arg);
            baseWriter.Write(s);
            onNewLine = false;
            currentColumn += s.Length;
        }

        public override void WriteLine(string s) 
        {
            OutputIndent();
            baseWriter.WriteLine(s);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine() 
        {
            OutputIndent();
            baseWriter.WriteLine();
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(bool value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(char value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(char[] buffer) 
        {
            OutputIndent();
            baseWriter.WriteLine(buffer);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(char[] buffer, int index, int count) 
        {
            OutputIndent();
            baseWriter.WriteLine(buffer, index, count);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(double value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(float value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(int value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(long value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(object value) 
        {
            OutputIndent();
            baseWriter.WriteLine(value);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(string format, object arg0) 
        {
            OutputIndent();
            baseWriter.WriteLine(format, arg0);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(string format, object arg0, object arg1) 
        {
            OutputIndent();
            baseWriter.WriteLine(format, arg0, arg1);
            indentPending = true;
            onNewLine = true;
            currentColumn = 0;
        }

        public override void WriteLine(string format, params object[] arg) 
        {
            OutputIndent();
            baseWriter.WriteLine(format, arg);
            indentPending = true;
            currentColumn = 0;
            onNewLine = true;
        }
    }
}
