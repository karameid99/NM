using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.General;
using DM.Core.Enums;
using DM.Infrastructure.Modules.Auth;
using DM.Infrastructure.Modules.Exhibition;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IExhibitionService _storeService;
        private readonly ExhibitionType _type;

        public StoreController(IExhibitionService exhibitionService)
        {
            _storeService = exhibitionService;
            _type = ExhibitionType.Store;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingDto dto)
        {
            var res = await _storeService.GetAll(dto, _type);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int Id)
        {
            var res = await _storeService.Get(Id, _type);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExhibitionDto dto)
        {
            await _storeService.Create(dto, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateExhibitionDto dto)
        {
            await _storeService.Update(dto, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _storeService.Delete(id, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var res = await _storeService.List(_type);
            return GetResponse(res);
        }
    }
}
