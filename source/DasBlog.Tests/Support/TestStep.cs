using System;
using System.Linq.Expressions;

namespace DasBlog.Tests.Support
{
	public class TestStep
	{
		private Expression<Action> action;
		private Expression<Func<bool>> func;
		private string description;
		public TestStep(Expression<Action> action, string description = null)
		{
			System.Diagnostics.Debug.Assert(action != null);
			this.description = description;
			this.action = action;
		}

		public TestStep(Expression<Func<bool>> func, string description = null)
		{
			System.Diagnostics.Debug.Assert(func != null);
			this.description = description;
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
			get { return description ?? (action == null ? (object) func : (object) action); }
		}
	}

	public class ActionStep : TestStep
	{
		public ActionStep(Expression<Action> action, string description = null) : base(action, description)
		{
		}
	}

	public class VerificationStep : TestStep
	{
		public VerificationStep(Expression<Func<bool>> func, string description = null) : base(func, description)
		{
		}
	}
}
