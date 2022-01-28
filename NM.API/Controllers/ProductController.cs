using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Products;
using DM.Core.DTOs.General;
using DM.Infrastructure.Modules.Auth;
using DM.Infrastructure.Modules.Product;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _ProductService;

        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsDto dto)
        {
            var res = await _ProductService.GetAll(dto);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int Id)
        {
            var res = await _ProductService.Get(Id);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            await _ProductService.Create(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateProductDto dto)
        {
            await _ProductService.Update(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _ProductService.Delete(id, GetCurrentUserId());
            return GetResponse();
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var res = await _ProductService.List();
            return GetResponse(res);
        }
    }
}
