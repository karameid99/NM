using AutoMapper;
using DM.Core.Entities;
using DM.Core.Enums;
using DM.Core.Exceptions;
using DM.Core.Movments;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task SingleProductMovmant(SingleProductMovmantDto dto, string userId)
        {
            switch (dto.MovmentActionType)
            {
                case MovmentActionType.AddMovemnt:
                    await SingleAddedMovmentType(dto.Id, dto.ShelfId, dto.Quantity, userId);
                    break;
                case MovmentActionType.NormalMovment:
                    await SingleNormalMovmentType(dto.ShelfId, dto.Quantity, dto.Id, userId, null);
                    break;
                case MovmentActionType.MoveToStore:
                    await SingleMoveToStoreMovmentType(dto.Quantity, dto.Id, userId);
                    break;
                case MovmentActionType.MoveToDamage:
                    await SingleMoveToDamageMovmentType(dto.Quantity, dto.Id, userId);
                    break;
                default:
                    break;
            }
        }
        private async Task SingleAddedMovmentType(int productId, int shelfId, int quantity, string userId)
        {
            ShelfProduct shelfProduct = null;
            shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.ProductId == productId && shelfId == x.ShelfId);
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
            await AddSingleProuctHistory(shelfProduct.Id, MovmentType.AddedMovemnt, OldQuantity, quantity, userId, null);
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
        private async Task SingleMoveToStoreMovmentType(int quantity, int currentShelfId, string userId)
        {
            var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.Shelf.Exhibition.Type == Core.Enums.ExhibitionType.Store);
            var ShelfId = shelfProduct != null ? shelfProduct.ShelfId : await _context.Shelfs.Where(x => x.Exhibition.Type == Core.Enums.ExhibitionType.Store).Select(x => x.Id).FirstOrDefaultAsync();
            await SingleNormalMovmentType(ShelfId, quantity, currentShelfId, userId, MovmentType.MoveToStore);
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
                && (!finished || x.Quantity == 0)
                && (finished || x.Quantity > 0)
                && (string.IsNullOrEmpty(dto.SearchKey) || x.Product.Name.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductExhibitionDto
                {
                    Id = c.Id,
                    Name = c.Product.Name,
                    ShelfId = c.ShelfId,
                    ShelfName = c.Shelf.Name,
                    LogoPath = c.Product.LogoPath,
                    Quantity = c.Quantity
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
        public async Task<List<ProductHistoryDto>> GetProductHistory(GetProductHistoryDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;
            return await _context.ProductHistories
                .Where(x => !x.IsDelete && x.NewShelfProductId == dto.Id
                && (!dto.FromDate.HasValue || x.CreatedAt.Value.Date >= dto.FromDate.Value.Date)
                && (!dto.ToDate.HasValue || x.CreatedAt.Value.Date <= dto.ToDate.Value.Date))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ProductHistoryDto
                {
                    FullName = c.UserFullName,
                    ActionDate = c.CreatedAt,
                    ToShelf = c.NewShelfProduct.Shelf.Name,
                    FromShelf = c.OldShelfProduct != null ? c.OldShelfProduct.Shelf.Name : "---",
                    NewQuantity = c.NewQuantity,
                    OldQuantity = c.OldQuantity,
                    DeferanceQunatity = c.AddedQuantity,
                    MovmentType = c.MovmentType.ToString()
                }).ToListAsync();
        }
        public async Task<List<ProductExhibitionDto>> GetProductFinished(GetProductFinishedDto dto)
        {
            return await GetProductExhibition(new GetProductExhibitionDto { Page = dto.Page, PerPage = dto.PerPage, SearchKey = dto.SearchKey }, true);
        }
        #endregion
    }
}
