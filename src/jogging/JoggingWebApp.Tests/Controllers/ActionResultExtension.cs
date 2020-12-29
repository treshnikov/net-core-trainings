using System;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace JoggingWebApp.Tests
{
    public static class ActionResultExtension
    {
        public static void ShouldBeBadRequestResult(this IActionResult actionResult)
        {
            if (actionResult is BadRequestResult || actionResult is BadRequestObjectResult)
            {
                return;
            }
            
            throw new InvalidOperationException($"actionResult should be of type {nameof(BadRequestResult)} or {nameof(BadRequestObjectResult)} but was {actionResult.GetType().Name}");
        }

        public static void ShouldBeOkResult(this IActionResult actionResult)
        {
            if (actionResult is OkResult || actionResult is OkObjectResult)
            {
                return;
            }
            
            throw new InvalidOperationException($"actionResult should be of type {nameof(OkResult)} or {nameof(OkObjectResult)} but was {actionResult.GetType().Name}");
        }
        
        public static void ShouldBeCreatedResult(this IActionResult actionResult)
        {
            if (actionResult is CreatedAtActionResult)
            {
                return;
            }
            
            throw new InvalidOperationException($"actionResult should be of type {nameof(CreatedAtActionResult)} but was {actionResult.GetType().Name}");
        }

        public static T GetCreatedObjectValue<T>(this IActionResult actionResult) where T: class
        {
            actionResult.ShouldBeOfType(typeof(CreatedAtActionResult));
            return GetValue<T>((CreatedAtActionResult)actionResult);
        }
        
        public static T GetOkObjectValue<T>(this IActionResult actionResult) where T: class
        {
            actionResult.ShouldBeOfType(typeof(OkObjectResult));
            return GetValue<T>((OkObjectResult) actionResult);
        }
        
        private static T GetValue<T>(ObjectResult actionResult) where T : class
        {
            var value = actionResult.Value;
            if (value == null)
            {
                return null;
            }
            
            var result = value as T;
            if (result == null)
            {
                throw new InvalidCastException($"Result should be of type {typeof(T)}");
            }

            return result;
        }
    }
}