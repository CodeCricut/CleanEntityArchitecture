using System.ComponentModel.DataAnnotations;

namespace CleanEntityArchitecture.Domain
{
	public class BaseUser : DomainEntity
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
