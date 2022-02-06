
using DM.Core.DTOs.General;
using DM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace NM.API.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{


		public override void OnActionExecuting(ActionExecutingContext context)
		{

			//Check model state in one place
			if (!context.ModelState.IsValid)
			{
				context.Result = new ObjectResult(new APIResponseError
				{
					Status = false,
					Message = "Missing Required Fields",
					Error = new BadRequestObjectResult(context.ModelState).Value
				})
				{
					StatusCode = (int)HttpStatusCode.BadRequest
				};
			}			
			}
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			// check exption in one place.
			if (!context.ExceptionHandled && context.Exception != null)
			{
				APIResponse response = null;
				var message = string.Empty;
				if (context.Exception is DMException)
				{
					response = new APIResponse()
					{
						Message = context.Exception.Message
					};
					context.Result = new ObjectResult(response)
					{
						StatusCode = (int)HttpStatusCode.BadRequest,
					};
				}
				else
				{

					response = new APIResponse()
					{
						Status = false,
					};

					response.Message = "Ohh Sory !! internal error occurred , please contact technical support";
					context.Result = new ObjectResult(response)
					{
						StatusCode = (int)HttpStatusCode.InternalServerError,
					};

				}

				context.ExceptionHandled = true;
			}
		}
	}

}
