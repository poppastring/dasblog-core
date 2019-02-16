using System;
using System.Linq;
using System.Reflection;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.Common
{
	/// <inheritdoc cref="ICacheFixer"/>
	public class CacheFixer : ICacheFixer
	{
		/// <inheritdoc cref="ICacheFixer.InvalidateCache"/>
		public void InvalidateCache(IBlogManager blogManager)
		{
			var categoryCacheType = GetType("newtelligence.DasBlog.Runtime.CategoryCache");
			if (categoryCacheType == null)
			{
				throw new Exception("failed to find type CategoryCache");
			}

			var changeNumberField =
				categoryCacheType.GetField("_changeNumber", BindingFlags.NonPublic | BindingFlags.Static);
			if (changeNumberField == null)
			{
				throw new Exception("Failed ot find static field _changeNumber on CategoryCache");
			}
			changeNumberField.SetValue(null, -1L);
			var dataServiceField = typeof(BlogManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.Name == "dataService").FirstOrDefault();
			if (dataServiceField == null)
			{
				throw new Exception("failed to find a field called dataSerice on BlogManager");
			}
			var dataService = dataServiceField.GetValue(blogManager);
			if (dataService == null)
			{
				throw new Exception("failed to find a value for dataSerice on BlogManager");
			}
			var dataManagerField = GetType("newtelligence.DasBlog.Runtime.BlogDataServiceXml").GetField("data", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (dataManagerField == null)
			{
				throw new Exception("failed to find a field called data on BlogDataServiceImpl");
			}
			var dataManager = dataManagerField.GetValue(dataService);
			if (dataManager == null)
			{
				throw new Exception("failed to find a value for the field 'data' on BlogDataServiceImpl");
			}

			var incrementEntryChangeMethod = GetType("newtelligence.DasBlog.Runtime.DataManager").GetMethod("IncrementEntryChange");
			if (incrementEntryChangeMethod == null)
			{
				throw new Exception("failed to find a method called IncrementEntryChangeMethod on DataManager");
			}

			incrementEntryChangeMethod.Invoke(dataManager, null);
		}
		/// <summary>
		/// returns a type from newtelligence.DasBlog.Runtime
		/// </summary>
		/// <param name="typeName">short name e.g. "BlogDataServiceXml"</param>
		/// <returns>typically a class</returns>
		private Type GetType(string typeName)
		{
			const string DasBlogAssemblyPrefix = "newtelligence.DasBlog.Runtime,";
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.FirstOrDefault(a => a.FullName.StartsWith(DasBlogAssemblyPrefix))
				?.GetType(typeName);
		}
	}
}
