using CleanEntityArchitecture.Authorization;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CleanEntityArchitecture.Helpers
{
	public static class MiddlewareExtensions
	{
		public static void UseJwtMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<JwtMiddleware>();
		}
	}
}
