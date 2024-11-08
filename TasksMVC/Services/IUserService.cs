using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;

namespace TasksMVC.Services
{
	public interface IUserService
	{
		string GetUserId();
	}

	public class UserService : IUserService
	{
		private HttpContext HttpContext;

		public UserService(IHttpContextAccessor httpContextAccessor) 
		{ 
			HttpContext = httpContextAccessor.HttpContext;
		}
		public string GetUserId()
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				var idClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

				return idClaim.Value;
			}
			else
			{
				throw new Exception("El usuario no está autenticado");
			}
		}
	}
}
