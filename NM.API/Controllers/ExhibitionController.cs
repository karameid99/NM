using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.General;
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

        public ExhibitionController(IExhibitionService exhibitionService)
        {
            _exhibitionService = exhibitionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingDto dto)
        {
            var res = await _exhibitionService.GetAll(dto);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int Id)
        {
            var res = await _exhibitionService.Get(Id);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExhibitionDto dto)
        {
            await _exhibitionService.Create(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateExhibitionDto dto)
        {
            await _exhibitionService.Update(dto, GetCurrentUserId());
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _exhibitionService.Delete(id, GetCurrentUserId());
            return GetResponse();
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var res = await _exhibitionService.List();
            return GetResponse(res);
        }
    }
}
