using CleanEntityArchitecture.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public interface IWriteEntityRepository<TEntity>
		where TEntity : DomainEntity
	{
		Task<TEntity> AddEntityAsync(TEntity entity);
		Task<IEnumerable<TEntity>> AddEntititesAsync(List<TEntity> entities);
		Task UpdateEntityAsync(int id, TEntity updatedEntity);
		Task SoftDeleteEntityAsync(int id);
		Task<bool> SaveChangesAsync();
	}
}
