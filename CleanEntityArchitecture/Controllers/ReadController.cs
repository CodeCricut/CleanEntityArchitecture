using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.EntityModelServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public abstract class ReadController<TEntity, TGetEntityModel> : ControllerBase, IReadController<TEntity, TGetEntityModel>
		where TEntity : DomainEntity
		where TGetEntityModel : GetModelDto
	{
		protected readonly IReadEntityService<TEntity, TGetEntityModel> _readService;

		public ReadController(IReadEntityService<TEntity, TGetEntityModel> readService)
		{
			_readService = readService;
		}

		[HttpGet]
		public virtual async Task<ActionResult<PagedList<TGetEntityModel>>> GetAsync([FromQuery] PagingParams pagingParams)
		{
			PagedList<TGetEntityModel> models = await _readService.GetAllEntityModelsAsync(pagingParams);
			return Ok(models);
		}

		[HttpGet("{key:int}")]
		public virtual async Task<ActionResult<TGetEntityModel>> GetByIdAsync(int key)
		{
			var model = await _readService.GetEntityModelAsync(key);

			return Ok(model);
		}
	}
}
