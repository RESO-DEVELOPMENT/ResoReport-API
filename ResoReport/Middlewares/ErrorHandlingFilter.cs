using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Reso.Sdk.Core.Custom;

namespace ResoReport.Middlewares
{
    public class ErrorHandlingFilter : IExceptionFilter
    {
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ErrorResponse)
            {
                ErrorResponse exception = ((ErrorResponse)context.Exception);
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = exception.Error.Code;
                context.Result = new JsonResult(exception.Error);
                return;
            }
            _logger.LogError(context.Exception.ToString());
#if DEBUG
            context.Result = new ObjectResult(new ErrorResponse((int)HttpStatusCode.InternalServerError, context.Exception.StackTrace))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
#else       
            context.Result = new ObjectResult(new ErrorResponse((int)HttpStatusCode.InternalServerError, "Opps, something went wrong!"))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
#endif

        }
    }
}
