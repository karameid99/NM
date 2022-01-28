using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Shelfs;
using DM.Core.DTOs.General;
using DM.Infrastructure.Modules.Auth;
using DM.Infrastructure.Modules.Shelf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class ShelfController : BaseController
    {
        private readonly IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllShelfsDto dto)
        {
            var res = await _shelfService.GetAll(dto);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int Id)
        {
            var res = await _shelfService.Get(Id);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShelfDto dto)
        {
            await _shelfService.Create(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShelfDto dto)
        {
            await _shelfService.Update(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _shelfService.Delete(id, GetCurrentUserId());
            return GetResponse();
        }
        [HttpGet]
        public async Task<IActionResult> List(int exhibitionId)
        {
            var res = await _shelfService.List(exhibitionId);
            return GetResponse(res);
        }
    }
}
