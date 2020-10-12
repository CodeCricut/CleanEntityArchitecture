using CleanEntityArchitecture.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanEntityArchitecture.Helpers
{
	public static class EntityServiceExtensions
	{
		public static IServiceCollection ConfigureCleanEntityArchitecture(this IServiceCollection services, IConfiguration configuration)
		{
			return services
				.Configure<JwtSettings>(options => configuration.GetSection("JwtSettings"));
		}
	}
}
