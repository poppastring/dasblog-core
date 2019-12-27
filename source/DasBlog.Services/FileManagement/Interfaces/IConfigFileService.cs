
namespace DasBlog.Services.FileManagement.Interfaces
{
	public interface IConfigFileService<T>
	{
		public bool SaveConfig(T config);
	}
}
