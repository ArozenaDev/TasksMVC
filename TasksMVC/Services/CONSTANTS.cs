using Microsoft.AspNetCore.Mvc.Rendering;

namespace TasksMVC.Services
{
	public class CONSTANTS
	{
		public const string AdminRole = "admin";

		public static readonly SelectListItem[] SupportedUICultures = new SelectListItem[] {
			new SelectListItem{Value = "es", Text = "Español"},
			new SelectListItem{Value = "en", Text = "English" }
		};
	}
}
