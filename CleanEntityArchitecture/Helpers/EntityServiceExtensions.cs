using CleanEntityArchitecture.EntityModelServices;
using CleanEntityArchitecture.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanEntityArchitecture.Helpers
{
	public static class EntityServiceExtensions
	{
		public static IServiceCollection AddEntityRepositories(this IServiceCollection services)
		{
			return services
				.AddScoped(typeof(IReadEntityRepository<>), typeof(ReadEntityRepository<>))
				.AddScoped(typeof(IWriteEntityRepository<>), typeof(WriteEntityRepository<>));
		}

		public static IServiceCollection AddEntityModelServices(this IServiceCollection services)
		{
			return services
				.AddScoped(typeof(IReadEntityService<,>), typeof(ReadEntityService<,>))
				.AddScoped(typeof(IWriteEntityService<,>), typeof(WriteEntityService<,>));
		}
	}
}
