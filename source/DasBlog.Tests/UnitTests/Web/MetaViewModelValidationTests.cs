using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DasBlog.Web.Models.AdminViewModels;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web
{
	public class MetaViewModelValidationTests
	{
		private static IList<ValidationResult> ValidateModel(MetaViewModel model)
		{
			var validationResults = new List<ValidationResult>();
			Validator.TryValidateObject(model, new ValidationContext(model), validationResults, validateAllProperties: true);
			return validationResults;
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void ValidateModel_InvalidTwitterCard_ReturnsValidationError()
		{
			var model = new MetaViewModel
			{
				TwitterCard = "player"
			};

			var errors = ValidateModel(model);

			Assert.Contains(errors, e => e.MemberNames.Contains(nameof(MetaViewModel.TwitterCard)));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void ValidateModel_InvalidTwitterHandle_ReturnsValidationError()
		{
			var model = new MetaViewModel
			{
				TwitterCard = "summary",
				TwitterSite = "invalid handle"
			};

			var errors = ValidateModel(model);

			Assert.Contains(errors, e => e.MemberNames.Contains(nameof(MetaViewModel.TwitterSite)));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void ValidateModel_ValidTwitterValues_ReturnsNoValidationErrors()
		{
			var model = new MetaViewModel
			{
				TwitterCard = "summary_large_image",
				TwitterSite = "@poppastring",
				TwitterCreator = "mark_downie"
			};

			var errors = ValidateModel(model);

			Assert.Empty(errors);
		}
	}
}
