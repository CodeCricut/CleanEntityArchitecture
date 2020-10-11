using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public abstract class ReadEntityRepository<TEntity> : IReadEntityRepository<TEntity>
		where TEntity : DomainEntity
	{
		private readonly DbContext _context;

		public ReadEntityRepository(DbContext context)
		{
			_context = context;
		}

		public virtual async Task<PagedList<TEntity>> GetEntitiesAsync(PagingParams pagingParams)
		{
			return await Task.Factory.StartNew(() =>
			{

				var withoutChildren = _context.Set<TEntity>().AsQueryable();
				var withChildren = IncludeChildren(withoutChildren);
				return PagedList<TEntity>.Create(withChildren, pagingParams);
			});
		}

		public async Task<IEnumerable<TEntity>> GetEntitiesAsync(IEnumerable<int> ids)
		{
			return await TaskHelper.RunConcurrentTasksAsync<int, TEntity>(ids.ToList(), id => GetEntityAsync(id));
		}

		public virtual async Task<bool> SaveChangesAsync()
		{
				return (await _context.SaveChangesAsync()) > 0;
		}

		public virtual async Task<bool> VerifyExistsAsync(int id)
		{
			return (await GetEntityAsync(id)) != null;
		}

		public virtual async Task<TEntity> GetEntityAsync(int id)
		{
			return await Task.Factory.StartNew(() =>
			{
				var withoutChildren = _context.Set<TEntity>().AsQueryable();
				var withChildren = IncludeChildren(withoutChildren);
				return withChildren.FirstOrDefault(a => a.Id == id);
			}); ;
		}

		public abstract IQueryable<TEntity> IncludeChildren(IQueryable<TEntity> queryable);

	}
}
