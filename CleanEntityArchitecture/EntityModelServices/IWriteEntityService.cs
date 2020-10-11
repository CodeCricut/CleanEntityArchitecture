using CleanEntityArchitecture.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public interface IWriteEntityService<TEntity, TPostModel>
		where TEntity : DomainEntity
		where TPostModel : PostModelDto
	{
		Task<TGetModel> PostEntityModelAsync<TGetModel>(TPostModel entityModel);
		Task PostEntityModelsAsync(List<TPostModel> entityModels);
		Task SoftDeleteEntityAsync(int id);
		Task<TGetModel> PutEntityModelAsync<TGetModel>(int id, TPostModel entityModel);
	}
}
