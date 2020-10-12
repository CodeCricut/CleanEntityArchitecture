using CleanEntityArchitecture.Domain;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Authorization
{
	public interface IUserAuth<TUser>
		where TUser : BaseUser
	{
		public Task<TUser> GetAuthenticatedUserAsync();
	}
}
