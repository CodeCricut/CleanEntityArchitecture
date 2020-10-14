using AutoMapper;
using CleanEntityArchitecture.Authorization;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public class AuthenticateUserService<TUser, TLoginModel, TGetUserModel> : IAuthenticateUserService<TLoginModel, TGetUserModel>
		where TUser : BaseUser
		where TLoginModel : LoginModel
		where TGetUserModel : GetModelDto
	{
		private readonly IUserLoginRepository<TLoginModel, TUser> _userLoginRepository;
		private readonly IMapper _mapper;
		private readonly IUserAuth<TUser> _userAuth;

		public AuthenticateUserService(IUserLoginRepository<TLoginModel, TUser> userLoginRepository,
			 IMapper mapper, IUserAuth<TUser> userAuth)
		{
			_userLoginRepository = userLoginRepository;
			_mapper = mapper;
			_userAuth = userAuth;
		}

		public virtual async Task<TGetUserModel> GetAuthenticatedReturnModelAsync()
		{
			var user = await _userAuth.GetAuthenticatedUserAsync();
			var privateReturnModel = _mapper.Map<TGetUserModel>(user);

			return privateReturnModel;
		}

		public virtual async Task<TGetUserModel> LoginAsync(TLoginModel model)
		{
			var user = await _userLoginRepository.GetUserByCredentialsAsync(model);

			// return null if not found
			if (user == null) return null;

			var getModel = _mapper.Map<TGetUserModel>(user);

			return getModel;
		}
	}
}
