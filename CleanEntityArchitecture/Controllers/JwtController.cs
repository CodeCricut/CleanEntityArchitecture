

using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.EntityModelServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public abstract class JwtController<TUser, TLoginModel> : ControllerBase, IJwtController<TLoginModel>
		where TUser : BaseUser
		where TLoginModel : LoginModel
	{
		private readonly IJwtService<TUser, TLoginModel> _jwtService;

		public JwtController(IJwtService<TUser, TLoginModel> jwtService)
		{
			_jwtService = jwtService;
		}

		[HttpPost]
		public virtual async Task<ActionResult<Jwt>> GetTokenAsync([FromBody] TLoginModel loginModel)
		{
			if (!ModelState.IsValid) throw new Exception();

			Jwt jwt = await _jwtService.GenerateJwtTokenAsync(loginModel);
			return Ok(jwt);
		}
	}
}
