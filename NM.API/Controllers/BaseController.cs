using DM.Core.DTOs.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NM.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class BaseController : ControllerBase
    {
        protected APIResponse _response;

        public BaseController()
        {
            _response = new APIResponse() ;
        }

        [NonAction]
        protected string GetCurrentUserId()
		{
			return User.FindFirst("UserId")?.Value;
		}

        #region Response

        #region Error

        /// <summary>
        /// Get General Error response
        /// </summary>
        /// <param name="responseEnum">Delete or NotFound</param>
        /// <returns>Ok response (Status=false, Data=null, Message=GeneralError)</returns>
        [NonAction]
        public IActionResult GetErrorResponse()
        {
            _response.Status = false;
            _response.Message = "Sorry! technical error please contact support";
            return Ok(_response);
        }

        /// <summary>
        /// Get Error response
        /// </summary>
        /// <param name="message">error Message </param>
        /// <returns>Ok response (Status=false, Data=null, Message=message)</returns>
        [NonAction]
        public IActionResult GetErrorResponse(string message)
        {
            _response.Status = false;
            _response.Message = message;
            return Ok(_response);
        }
        #endregion

        #region Success

        /// <summary>
        /// Get Success response with entity(es).
        /// </summary>
        /// <param name="data">Base entity to return it with response data</param>
        /// <returns>Ok response (Status=true, Data=data, Message=GeneralSucess)</returns>
        [NonAction]
        public IActionResult GetResponse(object data)
        {
            _response.Status = true;
            _response.Message = "Sucess";
            _response.Data = data;
            return Ok(_response);
        }

        /// <summary>
        /// Get General Sucess response
        /// </summary>
        /// <returns>Ok response (Status=false, Data=null, Message=GeneralSucess)</returns>
        [NonAction]
        public IActionResult GetResponse()
        {
            _response.Status = true;
            _response.Message = "Sucess";
            return Ok(_response);
        }
        #endregion

        #region Full
        /// <summary>
        /// Get Api response object
        /// </summary>
        /// <param name="data">object Data</param>
        /// <param name="message">reponse message</param>
        /// <param name="status">True if success, and false if there is any failer</param>
        /// <returns>Ok response (Status=status, Data=data, Message=message)</returns>
        [NonAction]
        public IActionResult GetResponse(object data, string message, bool status)
        {
            _response.Status = status;
            _response.Message = message;
            _response.Data = data;
            return Ok(_response);
        }

         [NonAction]
        public IActionResult GetPagingResponse(object data,int count, string message, bool status)
        {
            var response = new APIPagingResponse();
            response.Status = status;
            response.Message = message;
            response.Total = count;
            response.Data = data;
            return Ok(response);
        }
        #endregion

        #endregion
    }
}
