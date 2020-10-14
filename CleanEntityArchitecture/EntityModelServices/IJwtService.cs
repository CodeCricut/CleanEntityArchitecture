using CleanEntityArchitecture.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public interface IJwtService<TUser, TLoginModel>
		where TUser : BaseUser
		where TLoginModel : LoginModel
	{
		Task<Jwt> GenerateJwtTokenAsync(TLoginModel loginModel);
	}
}
