using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DasBlog.Services.Email
{
	public abstract class HostedService : IHostedService
	{
		private Task _executingTask;
		private CancellationTokenSource _cts;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			// Create a linked token so we can trigger cancellation outside of this token's cancellation
			_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			// Store the task we're executing
			_executingTask = ExecuteAsync(_cts.Token);

			// If the task is completed then return it, otherwise it's running
			return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			// Stop called without start
			if (_executingTask == null)
			{
				return;
			}

			// Signal cancellation to the executing method
			_cts.Cancel();

			// Wait until the task completes or the stop token triggers
			await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

			// Throw if cancellation triggered
			cancellationToken.ThrowIfCancellationRequested();
		}

		// Derived classes should override this and execute a long running method until 
		// cancellation is requested
		protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
	}
}
