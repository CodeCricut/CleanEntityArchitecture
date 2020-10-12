using AutoMapper;
using CleanEntityArchitecture.Domain;
using CleanEntityArchitecture.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.EntityModelServices
{
	public class ReadEntityService<TEntity, TGetModel> : IReadEntityService<TEntity, TGetModel>
		where TEntity : DomainEntity
		where TGetModel : GetModelDto
	{
		private readonly IMapper _mapper;
		private readonly IReadEntityRepository<TEntity> _readRepository;

		public ReadEntityService(IMapper mapper, IReadEntityRepository<TEntity> readRepository)
		{
			_mapper = mapper;
			_readRepository = readRepository;
		}

		public virtual async Task<PagedList<TGetModel>> GetAllEntityModelsAsync(PagingParams pagingParams)
		{
			var entityPagedList = (await _readRepository.GetEntitiesAsync(pagingParams));

			// convert to list of models
			List<TEntity> entityList = entityPagedList.ToList();
			var entityModelList = _mapper.Map<List<TGetModel>>(entityList);

			// return paged list of models
			return new PagedList<TGetModel>(entityModelList, entityPagedList.Count, pagingParams);
		}

		public virtual async Task<TGetModel> GetEntityModelAsync(int id)
		{
			TEntity entity = await _readRepository.GetEntityAsync(id);

			return _mapper.Map<TGetModel>(entity);
		}
	}
}
