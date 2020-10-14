using CleanEntityArchitecture.Authorization;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public class JwtService<TUser, TLoginModel> : IJwtService<TUser, TLoginModel>
		where TUser : BaseUser
		where TLoginModel : LoginModel
	{
		private readonly IUserLoginRepository<TLoginModel, TUser> _userLoginRepo;
		private readonly IJwtHelper _jwtHelper;

		public JwtService(IUserLoginRepository<TLoginModel, TUser> userLoginRepo,
			IJwtHelper jwtHelper)
		{
			_userLoginRepo = userLoginRepo;
			_jwtHelper = jwtHelper;
		}

		public async Task<Jwt> GenerateJwtTokenAsync(TLoginModel loginModel)
		{
			var user = await _userLoginRepo.GetUserByCredentialsAsync(loginModel);
			var token = _jwtHelper.GenerateJwtToken(user);
			return token;
		}
	}
}
