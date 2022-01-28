using AutoMapper;
using DM.Core.DTOs.General;
using DM.Core.DTOs.Products;
using DM.Core.Exceptions;
using DM.Infrastructure.Modules.Image;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Product
{
    public class ProductService : IProductService
    {
        private readonly DMDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        public ProductService(DMDbContext context, IMapper napper, IImageService imageService)
        {
            _context = context;
            _mapper = napper;
            _imageService = imageService;
        }

        public async Task Create(CreateProductDto dto, string userId)
        {
            var Product = _mapper.Map<DM.Core.Entities.Product>(dto);
            Product.CreatedBy = userId;
            if (dto.Logo != null) Product.LogoPath = await _imageService.Save(dto.Logo, "Images");

            await _context.Products.AddAsync(Product);
            await _context.SaveChangesAsync();
        }



        public async Task Delete(int id, string userId)
        {
            var Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (Product == null)
                throw new DMException("Products does't exists");
            if (Product.IsDelete)
                throw new DMException("Products already deleted");

            Product.IsDelete = true;
            _context.Products.Update(Product);
            await _context.SaveChangesAsync();
        }


        public async Task<ProductDto> Get(int id)
        {
            var Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (Product == null)
                throw new DMException("Products does't exists");
            if (Product.IsDelete)
                throw new DMException("Products already deleted");
            return _mapper.Map<ProductDto>(Product);
        }


        public async Task<List<ListItemDto>> List()
        {
            return await _context.Products
                    .Where(x => !x.IsDelete)
                    .Select(c => new ListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                    }).ToListAsync();
        }


        public async Task Update(UpdateProductDto dto, string userId)
        {
            var Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (Product == null)
                throw new DMException("Products does't exists");
            if (Product.IsDelete)
                throw new DMException("Products already deleted");

            Product.Name = dto.Name;
            Product.ProductNo = dto.ProductNo;
            Product.Description = dto.Description;
            if (dto.Logo != null) Product.LogoPath = await _imageService.Save(dto.Logo, "Images");

            _context.Products.Update(Product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetAll(GetAllProductsDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;

            return await _context.Products
                .Where(x => !x.IsDelete 
                && (string.IsNullOrEmpty(dto.SearchKey)
                || x.Name.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    LogoPath = c.LogoPath,
                    ProductNo = c.ProductNo
                }).ToListAsync();
        }
    }
}
