using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace DasBlog.Web.ModelBinding
{
	public class InvariantDoubleModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder? GetBinder(ModelBinderProviderContext context)
		{
			ArgumentNullException.ThrowIfNull(context);

			if (context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?))
			{
				return new InvariantDoubleModelBinder();
			}

			return null;
		}
	}
}
