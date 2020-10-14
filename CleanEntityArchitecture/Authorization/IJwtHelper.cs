using CleanEntityArchitecture.Domain;

namespace CleanEntityArchitecture.Authorization
{
	public interface IJwtHelper
	{
		Jwt GenerateJwtToken(BaseUser user);
	}
}
