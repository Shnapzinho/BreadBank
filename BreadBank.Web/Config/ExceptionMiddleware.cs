using System.Net;
namespace BreadBank.Web.Config
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware (RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception ex)
		{ 
			context.Response.ContentType = "text/plain";
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			return context.Response.WriteAsync(ex.Message);
		}
	}
}
