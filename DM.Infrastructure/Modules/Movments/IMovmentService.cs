using DM.Core.Movments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments
{
   public interface IMovmentService
    {
        Task SingleProductMovmant(SingleProductMovmantDto dto, string userId);
        Task<List<ProductExhibitionDto>> GetProductExhibition(GetProductExhibitionDto dto, bool finished = false);
        Task<List<ProductExhibitionDto>> GetProductStore(GetProductStoreDto dto);
        Task<List<ProductExhibitionDto>> GetProductDamaged(GetProductDamagedDto dto);
        Task<List<ProductExhibitionDto>> GetProductFinished(GetProductFinishedDto dto);
        Task<List<ProductHistoryDto>> GetProductHistory(GetProductHistoryDto dto);
        Task<GetDashboradDto> GetDashborad();
    }
}
