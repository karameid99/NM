using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Exhibition
{
    public interface IExhibitionService
    {
        Task<ExhibitionDto> Get(int id);
        Task<List<ExhibitionDto>> GetAll(PagingDto dto);
        Task Create(CreateExhibitionDto dto, string userId);
        Task Update(UpdateExhibitionDto dto, string userId);
        Task Delete(int id , string userId);
        Task<List<ListItemDto>> List();

    }
}
