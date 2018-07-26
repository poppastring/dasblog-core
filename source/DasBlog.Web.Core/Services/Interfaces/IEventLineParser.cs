namespace DasBlog.Core.Services.Interfaces
{
	public interface IEventLineParser
	{
		/// <summary>
		/// Used to format log entries for display on the Activity report
		/// </summary>
		/// <param name="eventLine">e.g
		/// <![CDATA[2018-07-18 14:38:08.591 +01:00 [Information] DasBlog.Web.Controllers.AccountController: SecuritySuccess :: myemail@myemail.com logged in successfully :: http://localhost:50432/Account/Login]]>
		/// </param>
		/// <returns>true + a complete event data display item if the line has the correct format else false=null</returns>
		(bool, EventDataDisplayItem) Parse(string eventLine);
	}
}
