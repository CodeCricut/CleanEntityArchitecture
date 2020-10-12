using CleanEntityArchitecture.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public interface IWriteController<TEntity, TPostModel, TGetModel>
		where TEntity : DomainEntity
		where TPostModel : PostModelDto
		where TGetModel : GetModelDto
	{
		Task<ActionResult<TGetModel>> PostAsync([FromBody] TPostModel postModel);
		Task<ActionResult> PostRangeAsync([FromBody] IEnumerable<TPostModel> postModels);
		Task<ActionResult<TGetModel>> Put(int key, [FromBody] TPostModel updateModel);
		Task<ActionResult> Delete(int key);
	}
}
