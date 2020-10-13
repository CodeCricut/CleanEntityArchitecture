using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public interface IAuthenticateUserService<TLoginModel, TGetModel>
	{
		public Task<TGetModel> LoginAsync(TLoginModel model);

		public Task<TGetModel> GetAuthenticatedReturnModelAsync();
	}
}
