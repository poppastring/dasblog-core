using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface IXmlRpcManager
    {
		string Invoke(Stream blob);
	}
}
