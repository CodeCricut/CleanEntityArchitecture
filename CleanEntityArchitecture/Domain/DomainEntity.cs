using System;
using System.Collections.Generic;
using System.Text;

namespace CleanEntityArchitecture.Domain
{
	public abstract class DomainEntity
	{
		public int Id { get; set; }
		public bool Deleted { get; set; }
	}
}
