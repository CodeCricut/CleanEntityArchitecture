using CleanEntityArchitecture.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public interface IJwtController<TLoginModel>
		where TLoginModel : LoginModel
	{
		public Task<ActionResult<Jwt>> GetTokenAsync([FromBody] TLoginModel loginModel);
	}
}
