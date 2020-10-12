using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public abstract class WriteEntityService<TEntity, TPostModel> : IWriteEntityService<TEntity, TPostModel>
		where TEntity : DomainEntity
		where TPostModel : PostModelDto
	{
		// todo: shouldn't always map to get model
		public abstract Task<TGetModel> PostEntityModelAsync<TGetModel>(TPostModel entityModel);

		public virtual async Task PostEntityModelsAsync(List<TPostModel> entityModels)
		{
			await TaskHelper.RunConcurrentTasksAsync(entityModels, async entityModel => await PostEntityModelAsync<TPostModel>(entityModel));
		}

		public abstract Task<TGetModel> PutEntityModelAsync<TGetModel>(int id, TPostModel entityModel);

		public abstract Task SoftDeleteEntityAsync(int id);
	}
}
