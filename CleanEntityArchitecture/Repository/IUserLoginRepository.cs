using CleanEntityArchitecture.Domain;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public interface IUserLoginRepository<TLoginModel, TUser>
		where TLoginModel : LoginModel
		where TUser : BaseUser
	{
		Task<TUser> GetUserByCredentialsAsync(TLoginModel loginModel);
	}
}
