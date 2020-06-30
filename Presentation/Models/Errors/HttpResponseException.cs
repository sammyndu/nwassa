using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nwassa.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Models.Errors
{
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }

    public class ErrorResponse
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error message for the inner exception
        /// </summary>
        public string InnerExceptionMessage { get; set; }

        /// <summary>
        /// Stack trace for the exception
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Additional data
        /// </summary>
        public System.Collections.IDictionary Data { get; set; }
    }

    public static class GlobalExceptionHandler
    {
        private static bool _sendErrorDetails;

        /// <summary>
        /// Uses custom implemention for global exception handling
        /// </summary>
        /// <param name="builder">ApplicationBuilder instance</param>
        /// /// <param name="logger">Logger instance</param>
        public static void HandleExceptions(this IApplicationBuilder builder)
        {
            _sendErrorDetails = true;

            builder.Run(HandleServerError);
        }

        private static async Task HandleServerError(HttpContext context)
        {

            var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (error == null)
            {
                return;
            }

            var accessControlOriginKey = "Access-Control-Allow-Origin";
            (var errorMessage, var statusCode) = GetErrorMessageAndStatusCode(error);
            var response = new ErrorResponse
            {
                Message = errorMessage,
            };

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            if (!context.Response.Headers.ContainsKey(accessControlOriginKey))
            {
                context.Response.Headers.Append(accessControlOriginKey, "*");
            }

            if (_sendErrorDetails)
            {
                response.Data = error.Data;
                response.StackTrace = error.StackTrace;
                response.InnerExceptionMessage = error.InnerException != null ? error.GetBaseException().Message : null;
            }
            // always send data for validationException
            else if (error is ValidationException)
            {
                response.Data = error.Data;
            }

            await context.Response.WriteAsync(response.ToJsonString(CasingType.Camel)).ConfigureAwait(false);
        }

        private static (string, HttpStatusCode) GetErrorMessageAndStatusCode(Exception exception)
        {
            var statusCode = default(HttpStatusCode);
            var defaultMsg = default(string);

            switch (exception)
            {
                case KeyNotFoundException kException:
                    statusCode = HttpStatusCode.NotFound;
                    defaultMsg = "The requested resource cannot be found";
                    break;

                case ArgumentException aExcption:
                case ValidationException vException:
                    statusCode = HttpStatusCode.BadRequest;
                    defaultMsg = "Invalid request";
                    break;

                case AccessViolationException avException:
                case UnauthorizedAccessException uaExption:
                    statusCode = HttpStatusCode.Unauthorized;
                    defaultMsg = "Access denied";
                    break;

                case NotImplementedException niException:
                    statusCode = HttpStatusCode.NotImplemented;
                    defaultMsg = "The requested resource is currently not available";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    defaultMsg = "Internal Server Error";
                    break;
            }

            return (GetErrorMessage(exception, defaultMsg), statusCode);
        }

        private static string GetErrorMessage(Exception exception, string defaultMessage)
        {
            if (exception is ValidationException)
            {
                return exception.Message;
            }

            var errorMsg = default(string);

            return errorMsg ?? (_sendErrorDetails ? exception.Message : defaultMessage);
        }
    }
}
