﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class SetupViewModel
	{
		[Required]
		[EmailAddress]
		[DisplayName("Admin email")]
		public string Email { get; set; }

		[Required]
		[DisplayName("Display name")]
		public string DisplayName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[DisplayName("Confirm password")]
		[Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
