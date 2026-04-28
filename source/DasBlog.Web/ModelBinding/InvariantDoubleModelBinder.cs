using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DasBlog.Web.ModelBinding
{
	public class InvariantDoubleModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			ArgumentNullException.ThrowIfNull(bindingContext);

			var modelName = bindingContext.ModelName;
			var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

			if (valueProviderResult == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

			var value = valueProviderResult.FirstValue;

			if (string.IsNullOrWhiteSpace(value))
			{
				return Task.CompletedTask;
			}

			if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
			{
				bindingContext.Result = ModelBindingResult.Success(result);
			}
			else
			{
				bindingContext.ModelState.TryAddModelError(modelName, $"The value '{value}' is not a valid number.");
			}

			return Task.CompletedTask;
		}
	}
}
