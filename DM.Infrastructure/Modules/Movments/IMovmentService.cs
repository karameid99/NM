using DM.Core.DTOs.Products;
using DM.Core.Movments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments
{
   public interface IMovmentService
    {
        Task SingleProductMovmant(SingleProductMovmantDto dto, string userId,bool isExternal = false);
        Task<List<ProductExhibitionDto>> GetProductExhibition(GetProductExhibitionDto dto, bool finished = false);
        Task<List<ProductExhibitionDto>> GetProductStore(GetProductStoreDto dto);
        Task<List<ProductExhibitionDto>> GetProductDamaged(GetProductDamagedDto dto);
        Task<List<ProductExhibitionDto>> GetProductFinished(GetProductFinishedDto dto);
        Task<List<ProductHistoryDto>> GetProductHistory(GetProductHistoryDto dto);
        Task<List<ProductHistoryReportDto>> GetallProductHistory(int id);
        Task<byte[]> GetRdlcPdfPackageAsBinaryDataAsync(string reportPath, int id, string name, List<ProductHistoryReportDto> products);
        Task<GetDashboradDto> GetDashborad();
        Task<List<SearchProductDto>> GetProducts(SearchProductInput input);
        Task DeleteProduct(int id);
    }
}
