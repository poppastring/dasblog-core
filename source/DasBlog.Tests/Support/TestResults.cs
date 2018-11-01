using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Tests.Support
{
	public class TestResults
	{
		public class Result
		{
			public string TestId { get; }
			public bool Success { get; }
			public string ErrorMessge { get; }
			public Result(string testid, bool success, string errorMessge)
			{
				this.TestId = testid;
				this.Success = success;
				this.ErrorMessge = errorMessge;
			}
		}
		public List<Result> Results { get; } = new List<Result>();
		public void Add(string testId, bool sucess, string errorMessage = null)
		{
			Results.Add(new Result(testId, sucess, errorMessage ?? string.Empty));
		}

		public bool TestPassed => Results.All(r => r.Success);
	}
}
