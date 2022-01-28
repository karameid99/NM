using DM.Core.Movments;
using DM.Infrastructure.Modules.Movments;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class MovmentController : BaseController
    {
        private readonly IMovmentService _MovmentService;

        public MovmentController(IMovmentService MovmentService)
        {
            _MovmentService = MovmentService;
        }


        [HttpPost]
        public async Task<IActionResult> SingleProductMovmant([FromBody] SingleProductMovmantDto dto)
        {
            await _MovmentService.SingleProductMovmant(dto, GetCurrentUserId());
            return GetResponse();
        }
       
    }
}
