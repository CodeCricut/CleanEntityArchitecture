using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Repository;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Authorization
{
	public class UserAuth<TUser> : IUserAuth<TUser>
		where TUser : BaseUser
	{
		private readonly HttpContext _httpContext;
		private readonly IReadEntityRepository<TUser> _readUserRepo;

		public UserAuth(IHttpContextAccessor contextAccessor, IReadEntityRepository<TUser> readUserRepo)
		{
			_httpContext = contextAccessor.HttpContext;
			_readUserRepo = readUserRepo;
		}

		public async Task<TUser> GetAuthenticatedUserAsync()
		{
			var userId = (int)_httpContext.Items["UserId"];
			var user = await _readUserRepo.GetEntityAsync(userId);
			return user;
		}
	}
}
