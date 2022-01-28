using DM.Core.DTOs.Shelfs;
using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Shelf
{
    public interface IShelfService
    {
        Task<ShelfDto> Get(int id);
        Task<List<ShelfDto>> GetAll(GetAllShelfsDto dto);
        Task Create(CreateShelfDto dto, string userId);
        Task Update(UpdateShelfDto dto, string userId);
        Task Delete(int id , string userId);
        Task<List<ListItemDto>> List(int exhibitionId);

    }
}
