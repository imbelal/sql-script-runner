using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SqlScriptRunner.ExceptionHandler;

public static class FunctionExceptionHandler
{
    public static async Task<IActionResult> ExecuteAsync(
        Func<Task<IActionResult>> action,
        HttpContext context, 
        ILogger logger)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError("An unhandled exception occurred: ", ex);
            // Create a response object with status code and message
            var statusCode = ex is RequestFailedException requestFailedException 
                ? requestFailedException.Status 
                : StatusCodes.Status500InternalServerError;
            // Create user friendly message.
            var message = ex is RequestFailedException requestFailedExceptionMessage 
                ? "Requested file doesn't exist." 
                : "An unexpected error occurred. Please try again later.";
            // Create response object.
            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };
            
            // Return the ObjectResult with status code and message
            return new ObjectResult(response);
        }
    }
}