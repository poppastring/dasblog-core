using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public interface IProcessTag
    {
        string ReplaceTag(string content);
    }
}
