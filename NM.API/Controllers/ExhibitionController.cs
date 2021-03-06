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
    public class ExhibitionController : BaseController
    {
        private readonly IExhibitionService _exhibitionService;
        private readonly ExhibitionType _type;
        public ExhibitionController(IExhibitionService exhibitionService)
        {
            _exhibitionService = exhibitionService;
            _type = ExhibitionType.Exhibition;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingDto dto)
        {
            var res = await _exhibitionService.GetAll(dto, _type);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int Id)
        {
            var res = await _exhibitionService.Get(Id, _type);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExhibitionDto dto)
        {
            await _exhibitionService.Create(dto, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateExhibitionDto dto)
        {
            await _exhibitionService.Update(dto, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _exhibitionService.Delete(id, GetCurrentUserId(), _type);
            return GetResponse();
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var res = await _exhibitionService.List(_type);
            return GetResponse(res);
        }
    }
}
