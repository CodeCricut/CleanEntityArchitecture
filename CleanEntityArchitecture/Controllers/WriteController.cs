using CleanEntityArchitecture.Authorization;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.EntityModelServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public abstract class WriteController<TEntity, TPostModel, TGetModel> : ControllerBase, IWriteController<TEntity, TPostModel, TGetModel>
		where TEntity : DomainEntity
		where TPostModel : PostModelDto
		where TGetModel : GetModelDto
	{
		protected readonly IWriteEntityService<TEntity, TPostModel> _writeService;

		public WriteController(IWriteEntityService<TEntity, TPostModel> writeService)
		{
			_writeService = writeService;
		}

		[HttpPost]
		[Authorize]
		public virtual async Task<ActionResult<TGetModel>> PostAsync([FromBody] TPostModel postModel)
		{
			if (!ModelState.IsValid) throw new Exception();
					// InvalidPostException(ModelState);

			var addedModel = await _writeService.PostEntityModelAsync<TGetModel>(postModel);

			return Ok(addedModel);
		}

		[HttpPost("range")]
		[Authorize]
		public virtual async Task<ActionResult> PostRangeAsync([FromBody] IEnumerable<TPostModel> postModels)
		{
			if (!ModelState.IsValid) throw new Exception();
			// InvalidPostException(ModelState);

			await _writeService.PostEntityModelsAsync(postModels.ToList());

			return Ok();
		}

		[HttpPut("{key:int}")]
		[Authorize]
		public virtual async Task<ActionResult<TGetModel>> Put(int key, [FromBody] TPostModel updateModel)
		{
			if (!ModelState.IsValid) throw new Exception();
			// InvalidPostException(ModelState);


			var updatedModel = await _writeService.PutEntityModelAsync<TGetModel>(key, updateModel);

			return Ok(updatedModel);
		}

		[HttpDelete("{key:int}")]
		[Authorize]
		public virtual async Task<ActionResult> Delete(int key)
		{
			if (!ModelState.IsValid) throw new Exception();
			// InvalidPostException(ModelState);

			await _writeService.SoftDeleteEntityAsync(key);

			return Ok();
		}
	}
}
