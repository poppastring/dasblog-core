using System.Text;

namespace newtelligence.DasBlog.Util
{
    public static class HttpHelper
    {
        public static string GetURLSafeString(string urlString)
        {
            if (urlString == null || urlString.Length == 0) return "";

            StringBuilder retVal = new StringBuilder(urlString.Length);

            if (urlString != null)
            {
                bool upper = false;
                bool pendingSpace = false;
                bool tag = false;

                for (int i = 0; i < urlString.Length; ++i)
                {
                    char c = urlString[i];

                    if (tag)
                    {
                        if (c == '>')
                        {
                            tag = false;
                        }
                    }
                    else if (c == '<')
                    {
                        // Discard everything between '<' and '>', inclusive.
                        // If no '>', just discard the '<'.
                        tag = (urlString.IndexOf('>', i) >= 0);
                    }

                    // Per RFC 2396 (URI syntax):
                    //  delims   = "<" | ">" | "#" | "%" | <">
                    //  reserved = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" | "$" | ","
                    // These characters should not appear in a URL
                    else if ("#%\";/?:@&=$,".IndexOf(c) >= 0)
                    {
                        continue;
                    }

                    else if (char.IsWhiteSpace(c))
                    {
                        pendingSpace = true;
                    }

                    // The marks may appear in URLs
                    //  mark = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
                    // as may all alphanumerics. (Tilde gets mangled by UrlEncode).
                    // Here we are more lenient than RFC 2396, as we allow
                    // all Unicode alphanumerics, not just the US-ASCII ones.
                    // SDH: Update: it's really safer and maintains more permalinks if we stick with A-Za-z0-9.
                    else if (char.IsLetterOrDigit(c) /* ||  "-_.!~*'()".IndexOf(c) >= 0 */ )
                    {
                        if (pendingSpace)
                        {
                            // Discard leading spaces
                            if (retVal.Length > 0)
                            {
                                
                            }
                            upper = true;
                            pendingSpace = false;
                        }

                        if (upper)
                        {
                            retVal.Append(char.ToUpper(c));
                            upper = false;
                        }
                        else
                        {
                            retVal.Append(c);
                        }
                    }
                }
            }

            return System.Web.HttpUtility.UrlEncode(retVal.ToString());
        }
    }
}
