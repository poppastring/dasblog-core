namespace DasBlog.Core.Services.Interfaces
{
	public interface ISiteRepairer
	{
		(bool result, string errorMessage)  RepairSite();
	}
}
