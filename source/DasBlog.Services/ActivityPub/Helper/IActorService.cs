using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub.Helper
{
	public interface IActorService
	{
		public Task SendSignedRequest(string document, Uri url, string publickeyid);

	}
}
