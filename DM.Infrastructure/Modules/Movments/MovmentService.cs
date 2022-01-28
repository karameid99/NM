using AutoMapper;
using DM.Core.Entities;
using DM.Core.Enums;
using DM.Core.Exceptions;
using DM.Core.Movments;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
using System;
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

        public Task MultipleProductMovmant(MultipleProductMovmantDto dto, string userId)
        {
            throw new NotImplementedException();
        }

        #region single Movment
        public async Task SingleProductMovmant(SingleProductMovmantDto dto, string userId)
        {
            switch (dto.MovmentActionType)
            {
                case MovmentActionType.AddMovemnt:
                    await SingleAddedMovmentType(dto.ProductId, dto.ShelfId, dto.Quantity, userId);
                    break;
                case MovmentActionType.NormalMovment:
                    if (!dto.Id.HasValue) throw new DMException("Currnet Shelf does't exists");
                    await SingleNormalMovmentType(dto.ProductId, dto.ShelfId, dto.Quantity, dto.Id.Value, userId, null);
                    break;
                case MovmentActionType.MoveToStore:
                    if (!dto.Id.HasValue) throw new DMException("Currnet Shelf does't exists");
                    await SingleMoveToStoreMovmentType(dto.Quantity, dto.Id.Value, userId);

                    break;
                case MovmentActionType.MoveToDamage:
                    if (!dto.Id.HasValue) throw new DMException("Currnet Shelf does't exists");
                    await SingleMoveToDamageMovmentType(dto.Quantity, dto.Id.Value, userId);
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
            await AddSingleProuctHistory(shelfProduct.Id, MovmentType.AddedMovemnt, OldQuantity, quantity, userId, null);
            await _context.SaveChangesAsync();
        }
        private async Task SingleNormalMovmentType(int productId, int shelfId, int quantity, int currentShelfId, string userId, MovmentType? publicMovmentType)
        {
            var newShelfProduct = await _context.ShelfProducts
                .Include(x => x.Shelf)
                .FirstOrDefaultAsync(x => x.ProductId == productId && shelfId == x.ShelfId);
            int newExhibitionId, oldExhibitionId;
            var OldQuantity = 0;
            if (newShelfProduct != null) newExhibitionId = newShelfProduct.Shelf.ExhibitionId;
            else
            {
                newShelfProduct = new ShelfProduct { ShelfId = shelfId, ProductId = productId };
                var shelf = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == shelfId);
                newExhibitionId = shelf.ExhibitionId;
            }

            var oldShelfProduct = await _context.ShelfProducts
                .FirstOrDefaultAsync(x => x.Id == currentShelfId);
            if (oldShelfProduct != null)
            {
                if (shelfId == oldShelfProduct.ShelfId) throw new DMException("You can't movement from Shelf to the same shelf");

                oldShelfProduct.Quantity -= quantity;
                newShelfProduct.Quantity += quantity;
                oldExhibitionId = oldShelfProduct.Shelf.ExhibitionId;
            }
            else
                throw new DMException("current Product does not exsist");

            _context.ShelfProducts.Update(newShelfProduct);
            _context.ShelfProducts.Update(oldShelfProduct);
            var movmentType = publicMovmentType ?? (newExhibitionId == oldExhibitionId ? MovmentType.InternalMovment : MovmentType.ExternalMovment);
            await AddSingleProuctHistory(newShelfProduct.Id, movmentType, OldQuantity, quantity, userId, null);
            await _context.SaveChangesAsync();

        }
        private async Task SingleMoveToStoreMovmentType(int quantity, int currentShelfId, string userId)
        {
            var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.Shelf.Exhibition.Type == Core.Enums.ExhibitionType.Store);
            await SingleNormalMovmentType(shelfProduct.ProductId, shelfProduct.ShelfId, quantity, currentShelfId, userId, MovmentType.MoveToStore);
        }
        private async Task SingleMoveToDamageMovmentType(int quantity, int currentShelfId, string userId)
        {
            var shelfProduct = await _context.ShelfProducts.FirstOrDefaultAsync(x => x.Shelf.Exhibition.Type == Core.Enums.ExhibitionType.Damaged);
            await SingleNormalMovmentType(shelfProduct.ProductId, shelfProduct.ShelfId, quantity, currentShelfId, userId, MovmentType.MoveToDamaged);
        }
        private async Task AddSingleProuctHistory(int newShelfProductId, MovmentType MovmentType, int OldQuantity, int NewQuantity, string userId, int? OldShelfProductId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var AddedQuantity = NewQuantity - OldQuantity;
            var history = new ProductHistory
            {
                AddedQuantity = AddedQuantity,
                OldQuantity = OldQuantity,
                NewQuantity = NewQuantity,
                MovmentType = MovmentType,
                NewShelfProductId = newShelfProductId,
                UserFullName = user.FirstName + " " + user.LastName,
                OldShelfProductId = OldShelfProductId,
                CreatedBy = userId
            };
            await _context.AddAsync(history);
        }
        #endregion

    }
}
