using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Email
{
	public class DataRefreshService : HostedService
	{
		// private readonly RandomStringProvider _randomStringProvider;

		public DataRefreshService()
		{
			// _randomStringProvider = randomStringProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				// await _randomStringProvider.UpdateString(cancellationToken);
				// Email here

				await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
			}
		}
	}
}
