using AutoMapper;
using DM.Core.DTOs.Products;
using DM.Core.Entities;
using DM.Core.Enums;
using DM.Core.Exceptions;
using DM.Core.Movments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using NM.Data.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments
{
    public class MovmentService : IMovmentService
    {
        private readonly DMDbContext _context;
        private readonly IMapper _mapper;

        public MovmentService(DMDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region single Movment
        public async Task SingleProductMovmant(SingleProductMovmantDto dto, string userId, bool isExternal = false)
        {
            switch (dto.MovmentActionType)
            {
                case MovmentActionType.AddMovemnt:
                    await SingleAddedMovmentType(dto.Id, dto.ShelfId, dto.Quantity, userId, isExternal);
                    break;
                case MovmentActionType.NormalMovment:
                    await SingleNormalMovmentType(dto.ShelfId, dto.Quantity, dto.Id, userId, null);
                    break;
                case MovmentActionType.MoveToStore:
                    await SingleNormalMovmentType(dto.ShelfId, dto.Quantity, dto.Id, userId,MovmentType.MoveToStore);
                    break;
                case MovmentActionType.MoveToDamage:
                    await SingleMoveToDamageMovmentType(dto.Quantity, dto.Id, userId);
                    break;
                case MovmentActionType.DeleteQuantity:
                    await SingleAddedMovmentType(dto.Id, dto.ShelfId, -dto.Quantity, userId, isExternal);
                    break;
                default:
                    break;
            }
        }
        private async Task SingleAddedMovmentType(int productId, int shelfId, int quantity, string userId, bool isExternal)
        {
            ShelfProduct shelfProduct = null;
            shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x =>
            (!isExternal ? x.Id == productId : x.ProductId == productId)
            && shelfId == x.ShelfId);
            var OldQuantity = 0;
            if (shelfProduct != null)
            {
                OldQuantity = shelfProduct.Quantity;
                shelfProduct.Quantity += quantity;
            }
            else
            {
                shelfProduct = new ShelfProduct
                {
                    ShelfId = shelfId,
                    Quantity = quantity,
                    ProductId = productId
                };
            }
            _context.ShelfProducts.Update(shelfProduct);
            await _context.SaveChangesAsync();
            await AddSingleProuctHistory(shelfProduct.Id, quantity > 0? MovmentType.AddedMovemnt : MovmentType.DeletedMovemnt, OldQuantity, quantity, userId, null);
            await _context.SaveChangesAsync();
        }
        private async Task SingleNormalMovmentType(int shelfId, int quantity, int currentShelfId, string userId, MovmentType? publicMovmentType)
        {
            int oldExhibitionId, OldQuantity = 0;

            var oldShelfProduct = await _context.ShelfProducts
                 .Include(x => x.Shelf)
                 .FirstOrDefaultAsync(x => x.Id == currentShelfId);

            if (oldShelfProduct != null && shelfId == oldShelfProduct.ShelfId) throw new DMException("You can't movement from Shelf to the same shelf");

            var OldShelfQuantity = oldShelfProduct.Quantity;
            if (quantity > OldShelfQuantity) throw new DMException("the moved quantity is qrather than exist quantity");

            var newShelfProduct = await _context.ShelfProducts
                .Include(x => x.Shelf)
                .FirstOrDefaultAsync(x =>
                x.ProductId == oldShelfProduct.ProductId
                && shelfId == x.ShelfId);

            if (newShelfProduct != null)
                OldQuantity = newShelfProduct.Quantity;
            else
                newShelfProduct = new ShelfProduct { ShelfId = shelfId, ProductId = oldShelfProduct.ProductId };

            int newExhibitionId = newShelfProduct.Shelf != null ? newShelfProduct.Shelf.ExhibitionId : await _context.Shelfs.Where(x => x.Id == shelfId).Select(x => x.ExhibitionId).FirstOrDefaultAsync();

            oldShelfProduct.Quantity -= quantity;
            newShelfProduct.Quantity += quantity;
            oldExhibitionId = oldShelfProduct.Shelf.ExhibitionId;

            var products = new List<ShelfProduct>() { newShelfProduct, oldShelfProduct };
            _context.ShelfProducts.UpdateRange(products);
            await _context.SaveChangesAsync();
            await History(quantity, currentShelfId, userId, publicMovmentType, oldExhibitionId, OldQuantity, OldShelfQuantity, newShelfProduct, newExhibitionId);

        }
        private async Task History(int quantity, int currentShelfId, string userId, MovmentType? publicMovmentType, int oldExhibitionId, int OldQuantity, int OldShelfQuantity, ShelfProduct newShelfProduct, int newExhibitionId)
        {
            var movmentType = publicMovmentType ?? (newExhibitionId == oldExhibitionId ? MovmentType.InternalMovment : MovmentType.ExternalMovment);
            await AddSingleProuctHistory(newShelfProduct.Id, movmentType, OldQuantity, quantity, userId, currentShelfId);
            await AddSingleProuctHistory(currentShelfId, movmentType, OldShelfQuantity, -quantity, userId, newShelfProduct.Id);
            await _context.SaveChangesAsync();
        }
        private async Task SingleMoveToDamageMovmentType(int quantity, int currentShelfId, string userId)
        {
            var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.Shelf.Exhibition.Type == Core.Enums.ExhibitionType.Damaged);
            var ShelfId = shelfProduct != null ? shelfProduct.ShelfId : await _context.Shelfs.Where(x => x.Exhibition.Type == Core.Enums.ExhibitionType.Damaged).Select(x => x.Id).FirstOrDefaultAsync();
            await SingleNormalMovmentType(ShelfId, quantity, currentShelfId, userId, MovmentType.MoveToDamaged);
        }
        private async Task AddSingleProuctHistory(int newShelfProductId, MovmentType MovmentType, int OldQuantity, int NewQuantity, string userId, int? OldShelfProductId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var currnetQuantity = NewQuantity + OldQuantity;
            var history = new ProductHistory
            {
                AddedQuantity = NewQuantity,
                OldQuantity = OldQuantity,
                NewQuantity = currnetQuantity,
                MovmentType = MovmentType,
                NewShelfProductId = newShelfProductId,
                UserFullName = user.FirstName + " " + user.LastName,
                OldShelfProductId = OldShelfProductId,
                CreatedBy = userId
            };
            await _context.AddAsync(history);
        }
        public async Task<List<ProductExhibitionDto>> GetProductExhibition(GetProductExhibitionDto dto, bool finished = false)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;
            return await _context.ShelfProducts
                .Where(x => !x.IsDelete
                && (!dto.ExhibitionId.HasValue || x.Shelf.ExhibitionId == dto.ExhibitionId)
                && (!dto.ShelfId.HasValue || x.ShelfId == dto.ShelfId)
                && (string.IsNullOrEmpty(dto.SearchKey) 
                || x.Product.ProductNo.Contains(dto.SearchKey) 
                || x.Product.Name.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductExhibitionDto
                {
                    Id = c.Id,
                    Name = c.Product.Name,
                    ShelfId = c.ShelfId,
                    ShelfName = c.Shelf.Name,
                    LogoPath = c.Product.LogoPath,
                    Quantity = c.Quantity,
                    ProductNo = c.Product.ProductNo,
                    ShelfNo = c.Shelf.ShelfNo
                }).ToListAsync();
        }
        public async Task<List<ProductExhibitionDto>> GetProductStore(GetProductStoreDto dto)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Type == Core.Enums.ExhibitionType.Store);
            return await GetProductExhibition(new GetProductExhibitionDto { ExhibitionId = exhibition.Id, Page = dto.Page, PerPage = dto.PerPage, SearchKey = dto.SearchKey });
        }
        public async Task<List<ProductExhibitionDto>> GetProductDamaged(GetProductDamagedDto dto)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Type == Core.Enums.ExhibitionType.Damaged);
            return await GetProductExhibition(new GetProductExhibitionDto { ExhibitionId = exhibition.Id, Page = dto.Page, PerPage = dto.PerPage, SearchKey = dto.SearchKey });
        }
        public async Task<byte[]> GetRdlcPdfPackageAsBinaryDataAsync(string reportPath, int id, string name, List<ProductHistoryReportDto> products)
        {
            var productShelfId = await _context.ProductHistories.Include(c => c.NewShelfProduct).Where(x => x.NewShelfProductId == id).Select(x => x.NewShelfProductId).FirstOrDefaultAsync();
            var product = await _context.ShelfProducts.Include(c => c.Product).Where(x => x.Id == productShelfId).Select(x => x.Product).FirstOrDefaultAsync();
            LocalReport report = new LocalReport();
            report.ReportPath = reportPath;
            ReportParameter[] param = new ReportParameter[3];

            param[0] = new ReportParameter("ProductNumber", product.ProductNo ?? "-");
            param[1] = new ReportParameter("ProductName", product.Name ?? "-");
            param[2] = new ReportParameter("ProductDescription", product.Description ?? "-");

            report.SetParameters(param);

            report.DataSources.Add(new ReportDataSource(name, products));

            return report.Render("PDF");

        }
        public async Task<List<ProductHistoryDto>> GetProductHistory(GetProductHistoryDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;
            var data =  await _context.ProductHistories
                .Where(x => !x.IsDelete && x.NewShelfProductId == dto.Id
                && (!dto.FromDate.HasValue || x.CreatedAt.Value.Date >= dto.FromDate.Value.Date)
                && (!dto.ToDate.HasValue || x.CreatedAt.Value.Date <= dto.ToDate.Value.Date))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductHistoryDto
                {
                    FullName = c.UserFullName,
                    ActionDate = c.CreatedAt,
                    ToShelf = c.AddedQuantity > 0 ? c.NewShelfProduct.Shelf.Name : c.OldShelfProduct.Shelf.Name,
                    FromShelf = c.AddedQuantity > 0 ? c.OldShelfProduct != null ? c.OldShelfProduct.Shelf.Name : "---" : c.NewShelfProduct.Shelf.Name,
                    NewQuantity = c.NewQuantity,
                    OldQuantity = c.OldQuantity,
                    DeferanceQunatity = c.AddedQuantity,
                    MovmentTypeEnum = c.MovmentType,
                }).ToListAsync();

            foreach (var item in data)
                item.MovmentType = GetEnumDescription(item.MovmentTypeEnum);

            return data;
        }
        public async Task<List<ProductExhibitionDto>> GetProductFinished(GetProductFinishedDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;
            return await _context.Products
                .Where(x => !x.IsDelete
                && !x.ShelfProducts.Any(c=> c.Quantity >0 && c.Shelf.Exhibition.Type == ExhibitionType.Exhibition)
                && (string.IsNullOrEmpty(dto.SearchKey)
                || x.ProductNo.Contains(dto.SearchKey)
                || x.Name.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductExhibitionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    LogoPath = c.LogoPath,
                    Quantity = 0,
                    ProductNo = c.ProductNo,
                }).ToListAsync();
        }
        public async Task<List<ProductHistoryReportDto>> GetallProductHistory(int id)
        {
            var data = await _context.ProductHistories
                .Where(x => x.NewShelfProductId == id)
                .Include(x => x.NewShelfProduct)
                .ThenInclude(x => x.Shelf)
                .Include(x => x.OldShelfProduct)
                .ThenInclude(x => x.Shelf)
                 .Select(c => new ProductHistoryReportDto
                 {
                     FullName = c.UserFullName,
                     ActionDate = c.CreatedAt.Value.ToString("dd/MM/yyyy mm:hh"),
                     ToShelf = c.AddedQuantity > 0 ? c.NewShelfProduct.Shelf.Name : c.OldShelfProduct.Shelf.Name,
                     FromShelf = c.AddedQuantity > 0 ? c.OldShelfProduct != null ? c.OldShelfProduct.Shelf.Name : "---" : c.NewShelfProduct.Shelf.Name,
                     NewQantity = c.NewQuantity,
                     OldQuantity = c.OldQuantity,
                     Deferance = c.AddedQuantity,
                     MovmentTypeEnum = c.MovmentType,
                 }).ToListAsync();

            foreach (var item in data)
                item.MovmantType = GetEnumDescription(item.MovmentTypeEnum);
            return data;
        }
        public async Task<GetDashboradDto> GetDashborad()
        {
            return new GetDashboradDto
            {
                CurrentProducts = await _context.Products.CountAsync(x => !x.IsDelete),
                DamagedProducts = await _context.ShelfProducts.Where(x => !x.IsDelete && x.Shelf.Exhibition.Type == ExhibitionType.Damaged && x.Quantity != 0).CountAsync(),
                Exhibitions = await _context.Exhibitions.CountAsync(x => x.Type == ExhibitionType.Exhibition && !x.IsDelete),
                Users = await _context.Users.CountAsync(x => !x.IsDelete && x.UserType == UserType.Supervisor),
                FinishedProducts = await _context.Products.Where(x => !x.IsDelete && !x.ShelfProducts.Any(c => c.Quantity > 0 && c.Shelf.Exhibition.Type == ExhibitionType.Exhibition)).CountAsync(),
                StoreProducts = await _context.Exhibitions.Where(x => !x.IsDelete && x.Type == ExhibitionType.Store).CountAsync(),
            };
        }
        public async Task<List<SearchProductDto>> GetProducts(SearchProductInput input)
        {
            var skipValue = (input.Page - 1) * input.PerPage;

            return await _context.ShelfProducts.Where(x =>
            !x.IsDelete && 
            x.Shelf.Exhibition.Type != ExhibitionType.Store
            && (string.IsNullOrEmpty(input.SearchKey)
            || x.Product.Name.Contains(input.SearchKey)
            || x.Product.ProductNo.Contains(input.SearchKey)
            || x.Shelf.Name.Contains(input.SearchKey)
            || x.Shelf.ShelfNo.Contains(input.SearchKey)
            || x.Shelf.Exhibition.Name.Contains(input.SearchKey)
            )).Select(x => new SearchProductDto
            {
                ExhibitionId = x.Shelf.ExhibitionId,
                ExhibitionName = x.Shelf.Exhibition.Name,
                LogoPath = x.Product.LogoPath,
                Id = x.Id,
                ProductName = x.Product.Name,
                ProductNo = x.Product.ProductNo,
                Quantity = x.Quantity,
                ShelfId = x.ShelfId,
                ShelfName = x.Shelf.Name,
                ShelfNo = x.Shelf.ShelfNo,
                IsStore = x.Shelf.Exhibition.Type == ExhibitionType.Store,
            }).OrderByDescending(x => x.Id).Skip(skipValue).Take(input.PerPage).ToListAsync();
        }

        private string GetEnumDescription(MovmentType value)
        {
            switch (value)
            {
                case MovmentType.AddedMovemnt:
                    return "Add quantity";
                case MovmentType.InternalMovment:
                    return "Transfering to onther shelf";
                case MovmentType.ExternalMovment:
                    return "Moving from wearhouse to another";
                case MovmentType.MoveToStore:
                    return "Move To Store";
                case MovmentType.MoveToDamaged:
                    return "Move To Damaged section";
                case MovmentType.DeletedMovemnt:
                    return "Delete quantity";
                default:
                    return "";

            }
        }

        public async Task DeleteProduct(int id)
        {
            var Product = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.Id == id);
            if (Product == null)
                throw new DMException("Products does't exists");
            if (Product.IsDelete)
                throw new DMException("Products already deleted");

            Product.IsDelete = true;
            _context.ShelfProducts.Update(Product);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
