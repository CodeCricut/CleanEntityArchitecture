using CleanEntityArchitecture.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanEntityArchitecture.Domain
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; private set; }
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;

		public PagedList(List<T> items, int count, PagingParams pagingParams)
		{
			TotalCount = count;
			PageSize = pagingParams.PageSize;
			CurrentPage = pagingParams.PageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pagingParams.PageSize);
			AddRange(items);
		}

		/// <summary>
		/// Create a new paged list out of a queryable resource collection.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pagingParams"></param>
		/// <returns></returns>
		public static PagedList<T> Create(IQueryable<T> source, PagingParams pagingParams)
		{
			var count = source.Count();
			var items = source.Paginate(pagingParams).ToList();
			return new PagedList<T>(items, count, pagingParams);
		}
	}
}
