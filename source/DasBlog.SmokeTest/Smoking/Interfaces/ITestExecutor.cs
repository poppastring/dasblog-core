﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DasBlog.SmokeTest.Smoking;

namespace DasBlog.SmokeTest.Smoking.Interfaces
{
	public interface ITestExecutor
	{
		void Execute(IList<TestStep> steps, TestResults testResults, [CallerMemberName] string testName = null);
	}
}
