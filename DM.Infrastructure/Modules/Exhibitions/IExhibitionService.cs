using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.General;
using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Exhibition
{
    public interface IExhibitionService
    {
        Task<ExhibitionDto> Get(int id, ExhibitionType type);
        Task<List<ExhibitionDto>> GetAll(PagingDto dto, ExhibitionType type);
        Task Create(CreateExhibitionDto dto, string userId, ExhibitionType type);
        Task Update(UpdateExhibitionDto dto, string userId, ExhibitionType type);
        Task Delete(int id , string userId, ExhibitionType type);
        Task<List<ListItemDto>> List(ExhibitionType type);

    }
}
