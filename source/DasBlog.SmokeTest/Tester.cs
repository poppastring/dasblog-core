using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using DasBlog.SmokeTest.Common;
using static DasBlog.SmokeTest.Common.Utils;
using DasBlog.SmokeTest.Dom;
using DasBlog.SmokeTest.Interfaces;
using OpenQA.Selenium.Firefox;

namespace DasBlog.SmokeTest
{
	internal class Tester : ITester
	{
		private readonly IBrowser browser;
		public TestResults Results { get; } = new TestResults();
		private readonly Pages pages;
		public Tester(IBrowser browser)
		{
			this.browser = browser;
			pages = new Pages(browser);
		}

		public void Test()
		{
			browser.Init();
			Login_WithBlankPassword_Fails();
			Thread.Sleep(10000);
		}

		private void Login_WithBlankPassword_Fails()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Login.Goto())
				, new TestStep(() => pages.Login.IsDisplayed())
				, new TestStep(() => pages.Login.LoginButton.Click())
				, new TestStep(() => pages.Login.Password.Text.ToLower().Contains("the password field is required"))
				, new TestStep(() => pages.Login.IsDisplayed())
			};
			ExecuteTestSteps(testSteps);
		}

		private void ExecuteTestSteps(List<TestStep> testSteps, [CallerMemberName]string testName = null)
		{
			foreach (var step in testSteps)
			{
				switch (step.Value)
				{
					case Action action:
						action();
						break;
					case Func<bool> func:
						bool result = func();
						if (!result)
						{
							Results.Add(testName, false, (step.Description as Expression<Func<bool>>).ToString());
							return;
						}
						break;
					default:
						throw new Exception("Unknow test step encountered");
				}
			}
			Results.Add(testName, false, string.Empty);
		}

		public void Dispose()
		{
			browser?.Dispose();
		}
	}


	public class TestStep
	{
		private Expression<Action> action;
		private Expression<Func<bool>> func;
		public TestStep(Expression<Action> action)
		{
			System.Diagnostics.Debug.Assert(action != null);
			this.action = action;
		}

		public TestStep(Expression<Func<bool>> func)
		{
			System.Diagnostics.Debug.Assert(func != null);
			this.func = func;
		}
		public object Value
		{
			get
			{
				return action == null ? (object)func?.Compile() : (object)action?.Compile();
			}
		}

		public object Description
		{
			get { return action == null ? (object) func : (object) action; }
		}
	}
}
