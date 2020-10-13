using CleanEntityArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public class UserLoginRepository<TLoginModel, TUser> : IUserLoginRepository<TLoginModel, TUser>
		where TLoginModel : LoginModel
		where TUser : BaseUser
	{
		private readonly DbContext _context;
		private readonly IReadEntityRepository<TUser> _readUserRepo;

		public UserLoginRepository(DbContext context, IReadEntityRepository<TUser> readUserRepo)
		{
			_context = context;
			_readUserRepo = readUserRepo;
		}

		public async Task<TUser> GetUserByCredentialsAsync(TLoginModel loginModel)
		{
			return await Task.Factory.StartNew(() =>
			{
				var withoutChildren = _context.Set<TUser>().AsQueryable();
				var withChildren = _readUserRepo.IncludeChildren(withoutChildren);

				return withChildren.FirstOrDefault(u => u.Username == loginModel.Username 
				&& u.Password == loginModel.Password);
			});
		}
	}
}
