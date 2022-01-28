using DM.Core.DTOs.Products;
using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Product
{
    public interface IProductService
    {
        Task<ProductDto> Get(int id);
        Task<List<ProductDto>> GetAll(GetAllProductsDto dto);
        Task Create(CreateProductDto dto, string userId);
        Task Update(UpdateProductDto dto, string userId);
        Task Delete(int id , string userId);
        Task<List<ListItemDto>> List();

    }
}
