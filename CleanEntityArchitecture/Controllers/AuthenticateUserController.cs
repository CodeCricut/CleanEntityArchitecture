using CleanEntityArchitecture.Authorization;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.EntityModelServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public abstract class AuthenticateUserController<TGetUserModel, TLoginModel>
		: ControllerBase, IAuthenticateUserController<TGetUserModel, TLoginModel>
		where TGetUserModel : GetModelDto
		where TLoginModel : LoginModel
	{
		private readonly IAuthenticateUserService<LoginModel, TGetUserModel> _userAuthService;

		public AuthenticateUserController(IAuthenticateUserService<LoginModel, TGetUserModel> userAuthService)
		{
			_userAuthService = userAuthService;
		}

		[HttpPost("login")]
		public virtual async Task<ActionResult<TGetUserModel>> LoginAsync([FromBody] TLoginModel loginModel)
		{
			var user = await _userAuthService.LoginAsync(loginModel);

			if (user == null) throw new Exception("Invalid username or password.");
			// NotFoundException("Username or password is incorrect.");

			return Ok(user);
		}

		[HttpGet("Me")]
		[JwtAuthorize]
		public virtual async Task<ActionResult<TGetUserModel>> GetPrivateUserAsync()
		{
			var privateUser = await _userAuthService.GetAuthenticatedReturnModelAsync();

			if (privateUser == null) return StatusCode(StatusCodes.Status500InternalServerError);

			return Ok(privateUser);
		}
	}
}
