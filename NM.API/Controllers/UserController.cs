using DM.Core.DTOs.Auth;
using DM.Core.DTOs.General;
using DM.Infrastructure.Modules.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingDto dto)
        {
            var res = await _authService.GetAll(dto);
            return GetResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(string Id)
        {
            var res = await _authService.Get(Id);
            return GetResponse(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            await _authService.Create(dto);
            return GetResponse();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateUserDto dto)
        {
            await _authService.Update(dto);
            return GetResponse();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _authService.Delete(id);
            return GetResponse();
        }
    }
}
