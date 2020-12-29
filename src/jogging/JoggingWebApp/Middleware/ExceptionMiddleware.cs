using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLogic.Exceptions;
using BusinessLogic.FilterParser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JoggingWebApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
 
        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }
 
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadRequestException ex)
            {
                await WriteResponse(httpContext, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await WriteResponse(httpContext, HttpStatusCode.NotFound, ex.Message);
            }
            catch (FilterException ex)
            {
                await WriteResponse(httpContext, HttpStatusCode.BadRequest, "Filter error: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                await WriteResponse(httpContext, HttpStatusCode.InternalServerError, null);
            }
        }

        private Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            if (string.IsNullOrWhiteSpace(message))
            {
                return Task.CompletedTask;
            }
            
            var body = new
            {
                Message = message
            };
            
            return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
        }
    }}