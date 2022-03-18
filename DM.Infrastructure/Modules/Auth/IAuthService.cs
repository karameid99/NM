using DM.Core.DTOs.Auth;
using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Auth
{
    public interface IAuthService
    {
        Task<UserLoginDto> Login(LoginDto dto);
        Task<UserDto> Get(string id);
        Task<List<UserDto>> GetAll(PagingDto dto);
        Task Create(CreateUserDto dto);
        Task Update(UpdateUserDto dto);
        Task Delete(string id);
        Task ChangeAdminPassword(ChangePasswordAdmindto dto, string userId);

    }
}
