using AutoMapper;
using CleanEntityArchitecture.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanEntityArchitecture.Authorization
{
	/// <summary>
	/// The custom JWT middleware checks if there is a token in 
	/// the request Authorization header, and if so attempts to:
	///
	/// <list type="bullet">
	///		<item>Validate the token</item>
	///		<item> Extract the user id from the token</item>
	///		<item>Attach the authenticated user to the current HttpContext.Items 
	///		collection to make it accessible within the scope of the current request</item>
	/// </list>
	/// 
	/// If no valid JWT token is provided, then no user will be attached to the context, and the 
	/// <see cref="Helpers.Attributes.AuthorizeAttribute"/> will filter out any requests that require authorization.
	/// </summary>
	public class JwtMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly JwtSettings _jwtSettings;

		public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
		{
			_next = next;
			_jwtSettings = jwtSettings.Value;
		}


		public async Task Invoke(HttpContext context)
		{
			var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

			if (token != null)
				AttachUserIdToContext(context, token);

			await _next(context);
		}

		private void AttachUserIdToContext(HttpContext context, string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_jwtSettings.JwtSecret);
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;
				var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

				// attach user to context on successful jwt validation
				context.Items["UserId"] = userId;
			}
			catch
			{
				// do nothing if jwt validation fails
				// user is not attached to context so request won't have access to secure routes
			}
		}
	}
}
