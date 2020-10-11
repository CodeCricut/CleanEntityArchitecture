using CleanEntityArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Repository
{
	public abstract class WriteEntityRepository<TEntity> : IWriteEntityRepository<TEntity>
		where TEntity : DomainEntity
	{
		private readonly DbContext _context;

		public WriteEntityRepository(DbContext context)
		{
			_context = context;
		}

		public virtual async Task<TEntity> AddEntityAsync(TEntity entity)
		{
				var addedEntity = (await Task.Run(() => _context.Set<TEntity>().Add(entity))).Entity;
				return addedEntity;
		}

		public virtual async Task<IEnumerable<TEntity>> AddEntititesAsync(List<TEntity> entities)
		{
			return await Task.Factory.StartNew(() =>
			{
				_context.Set<TEntity>().AddRange(entities);
				return entities;
			});
		}


		public virtual async Task UpdateEntityAsync(int id, TEntity updatedEntity)
		{
			await Task.Factory.StartNew(() =>
			{
				var local = _context.Set<TEntity>().Local.FirstOrDefault(x => x.Id == id);
				if (local != null) _context.Entry(local).State = EntityState.Detached;

				updatedEntity.Id = id;
				_context.Entry(updatedEntity).State = EntityState.Modified;
			});
		}

		public virtual async Task SoftDeleteEntityAsync(int id)
		{
			var entity = await _context.Set<TEntity>().FindAsync(id);
			entity.Deleted = true;
			await UpdateEntityAsync(id, entity);
		}


		public virtual async Task<bool> SaveChangesAsync()
		{
				return (await _context.SaveChangesAsync()) > 0;
		}
	}
}
