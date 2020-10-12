using CleanEntityArchitecture.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Controllers
{
	public interface IReadController<TEntity, TGetModel>
		where TEntity : DomainEntity
		where TGetModel : GetModelDto
	{
		Task<ActionResult<PagedList<TGetModel>>> GetAsync([FromQuery] PagingParams pagingParams);
		Task<ActionResult<TGetModel>> GetByIdAsync(int key);
	}
}
