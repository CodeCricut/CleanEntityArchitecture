using CleanEntityArchitecture.Domain;

namespace CleanEntityArchitecture.Authorization
{
	public interface IJwtHelper
	{
		string GenerateJwtToken(BaseUser user);
	}
}
