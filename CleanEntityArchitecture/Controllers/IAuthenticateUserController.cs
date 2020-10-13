using CleanEntityArchitecture.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public interface IAuthenticateUserController<TGetUserModel, TLoginModel>
		where TGetUserModel : GetModelDto
		where TLoginModel : LoginModel
	{
		public Task<ActionResult<TGetUserModel>> LoginAsync([FromBody] TLoginModel model);
		public Task<ActionResult<TGetUserModel>> GetPrivateUserAsync();
	}
}
