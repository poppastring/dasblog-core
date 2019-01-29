using System;

namespace newtelligence.DasBlog.Runtime.Html.Formatting
{
    [Flags]
    internal enum FormattingFlags 
    {
        None = 0,
        Inline = 0x1,
        NoIndent = 0x2,
        NoEndTag =  0x4,
        PreserveContent = 0x8,
        Xml = 0x10,
        Comment = 0x20,
        AllowPartialTags = 0x40
    }
}
