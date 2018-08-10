using System.Collections.Generic;

namespace DasBlog.Tests.Support
{
	public class TestResults
	{
		public class Result
		{
			public string TestId { get; private set; }
			public bool Success { get; private set; }
			public string ErrorMessge { get; private set; }
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
	}
}
