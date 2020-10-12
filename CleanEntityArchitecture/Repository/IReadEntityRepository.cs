using CleanEntityArchitecture.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public interface IReadEntityRepository<TEntity>
		where TEntity : DomainEntity
	{
		Task<TEntity> GetEntityAsync(int id);
		Task<PagedList<TEntity>> GetEntitiesAsync(PagingParams pagingParams);
		Task<IEnumerable<TEntity>> GetEntitiesAsync(IEnumerable<int> ids);
		Task<bool> SaveChangesAsync();
		Task<bool> VerifyExistsAsync(int id);
		IQueryable<TEntity> IncludeChildren(IQueryable<TEntity> queryable);
	}
}
