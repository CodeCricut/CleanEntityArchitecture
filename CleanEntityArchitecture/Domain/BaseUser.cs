﻿using System.ComponentModel.DataAnnotations;

namespace CleanEntityArchitecture.Domain
{
	public abstract class BaseUser : DomainEntity
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
