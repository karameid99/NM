using DM.Core.DTOs.Products;
using DM.Core.Movments;
using DM.Infrastructure.Modules.Movments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class MovmentController : BaseController
    {
        private readonly IMovmentService _MovmentService;
        private readonly IWebHostEnvironment environment;

        public MovmentController(IMovmentService MovmentService, IWebHostEnvironment environment)
        {
            _MovmentService = MovmentService;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductExhibition([FromQuery] GetProductExhibitionDto dto)
        {
            var res = await _MovmentService.GetProductExhibition(dto);
            return GetResponse(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductFinished([FromQuery] GetProductFinishedDto dto)
        {
            var res = await _MovmentService.GetProductFinished(dto);
            return GetResponse(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDamaged([FromQuery] GetProductDamagedDto dto)
        {
            var res = await _MovmentService.GetProductDamaged(dto);
            return GetResponse(res);
        }

        [HttpPost]
        public async Task<IActionResult> SingleProductMovmant([FromBody] SingleProductMovmantDto dto)
        {
            await _MovmentService.SingleProductMovmant(dto, GetCurrentUserId());
            return GetResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductHistory([FromQuery] GetProductHistoryDto dto)
        {
            var res = await _MovmentService.GetProductHistory(dto);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetDashborad()
        {
            var res = await _MovmentService.GetDashborad();
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Print(int id) 
        {
            var reportPath = environment.WebRootPath + "/Invoice.rdlc";

            ArrayList data = new ArrayList();
            ArrayList data1 = new ArrayList();

            data.Add("ProductHistory");

            data1.Add(await _MovmentService.GetallProductHistory(id));

           var file =  await _MovmentService.GetRdlcPdfPackageAsBinaryDataAsync(reportPath,id, "ProductHistory", await _MovmentService.GetallProductHistory(id));

            return File(file, "Application/pdf");
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] SearchProductInput input)
        {
            var res = await _MovmentService.GetProducts(input);
            return GetResponse(res);
        }
         [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _MovmentService.DeleteProduct(id);
            return GetResponse();
        }

    }
}
