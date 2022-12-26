namespace DasBlog.Web.Models.ActivityPubModels
{
	public class UserViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string first { get; set; }

#pragma warning disable IDE1006 // Naming Styles
		public string @context { get; set; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
