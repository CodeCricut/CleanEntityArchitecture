using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanEntityArchitecture.Domain
{
	public class PagingParams
	{
		[Range(1, int.MaxValue)]
		public int PageNumber { get; set; } = 1;
		[Range(1, 100)]
		public int PageSize { get; set; } = 10;
	}
}
