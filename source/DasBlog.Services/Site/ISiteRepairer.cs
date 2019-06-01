namespace DasBlog.Services.Site
{
	public interface ISiteRepairer
	{
		(bool result, string errorMessage)  RepairSite();
	}
}
