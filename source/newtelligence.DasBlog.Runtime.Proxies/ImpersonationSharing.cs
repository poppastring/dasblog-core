/// <summary>
/// Provides help to share identity between threads.
/// http://aspalliance.com/articleViewer.aspx?aId=650&vId=1&pId
/// </summary>
/// System.Security.Principal.WindowsImpersonationContext wi = Impersonation.Impersonate();
/// do some stuff
/// wi.Undo()

namespace newtelligence.DasBlog.Runtime.Proxies
{
	public sealed class Impersonation
	{
		#region Constructor
		/// <summary>
		/// Hidden constructor to prevent class initialization, i.e.,
		/// to create a static class.
		/// </summary>
		private Impersonation() {}
		#endregion

		#region ApplicationIdentity
		private static System.Security.Principal.WindowsIdentity applicationIdentity;
		/// <summary>
		/// Gets the current application identity.
		/// </summary>
		/// <remarks>If the identity has not been set, it will return 
		/// <see cref="System.Security.Principal.WindowsIdentity.GetCurrent"/>.</remarks>
		public static System.Security.Principal.WindowsIdentity ApplicationIdentity
		{
			get
			{
				if (applicationIdentity == null)
					applicationIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
				return applicationIdentity;
			}
			set
			{
				applicationIdentity = value;
			}
		}
		#endregion

		#region Impersonate
		/// <summary>
		/// Impersonates the identity specified by <see cref="ApplicationIdentity"/>.
		/// </summary>
		public static System.Security.Principal.WindowsImpersonationContext Impersonate()
		{
			System.Security.Principal.WindowsPrincipal newPrin = 
				new System.Security.Principal.WindowsPrincipal(ApplicationIdentity);
			System.Threading.Thread.CurrentPrincipal = newPrin;
			return ApplicationIdentity.Impersonate();
		}
		#endregion
	}
}