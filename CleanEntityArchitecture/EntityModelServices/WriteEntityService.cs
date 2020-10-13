using AutoMapper;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Helpers;
using CleanEntityArchitecture.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public class WriteEntityService<TEntity, TPostModel> : IWriteEntityService<TEntity, TPostModel>
		where TEntity : DomainEntity
		where TPostModel : PostModelDto
	{
		protected readonly IMapper _mapper;
		protected readonly IWriteEntityRepository<TEntity> _writeRepo;
		protected readonly IReadEntityRepository<TEntity> _readRepo;

		public WriteEntityService(IMapper mapper, IWriteEntityRepository<TEntity> writeRepo, IReadEntityRepository<TEntity> readRepo)
		{
			_mapper = mapper;
			_writeRepo = writeRepo;
			_readRepo = readRepo;
		}

		public virtual async Task<TGetModel> PostEntityModelAsync<TGetModel>(TPostModel entityModel)
		{
			var entity = _mapper.Map<TEntity>(entityModel);

			var addedEntity = await _writeRepo.AddEntityAsync(entity);
			await _writeRepo.SaveChangesAsync();

			return _mapper.Map<TGetModel>(addedEntity);
		}

		public virtual async Task PostEntityModelsAsync(List<TPostModel> entityModels)
		{
			await TaskHelper.RunConcurrentTasksAsync(entityModels, async entityModel => await PostEntityModelAsync<TPostModel>(entityModel));
		}

		public virtual async Task<TGetModel> PutEntityModelAsync<TGetModel>(int id, TPostModel entityModel)
		{
			// Verify eneity exists
			if (!await _readRepo.VerifyExistsAsync(id)) throw new Exception("Could not find an entity with the given ID to update.");

			var updatedEntity = _mapper.Map<TEntity>(entityModel);

			// Update and save
			await _writeRepo.UpdateEntityAsync(id, updatedEntity);
			await _writeRepo.SaveChangesAsync();

			// Return updated entity
			return _mapper.Map<TGetModel>(updatedEntity);
		}

		public virtual async Task SoftDeleteEntityAsync(int id)
		{
			// Soft delete and save
			await _writeRepo.SoftDeleteEntityAsync(id);
			await _writeRepo.SaveChangesAsync();
		}
	}
}
