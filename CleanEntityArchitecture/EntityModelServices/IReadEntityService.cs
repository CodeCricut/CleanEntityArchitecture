using CleanEntityArchitecture.Domain;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public interface IReadEntityService<TEntity, TGetModel>
		where TEntity : DomainEntity
		where TGetModel : GetModelDto
	{
		Task<TGetModel> GetEntityModelAsync(int id);
		Task<PagedList<TGetModel>> GetAllEntityModelsAsync(PagingParams pagingParams);
	}
}
