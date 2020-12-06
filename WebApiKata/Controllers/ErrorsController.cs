using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WebApiKata.Exceptions;
using WebApiKata.ResourceModels;

namespace WebApiKata.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [Route("error")]
        public ErrorResponseModel Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            Response.StatusCode = (int)GetHttpStatusCode(exception);
            _logger.LogError(exception, exception.Message);

            return new ErrorResponseModel() { ErrorMessage = exception.Message };
        }

        private HttpStatusCode GetHttpStatusCode(Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // Internal Server Error by default

            if (exception != null)
            {
                if (exception is BadApiRequestException)
                {
                    code = HttpStatusCode.BadRequest;
                }
            }
            return code;
        }
    }

}
