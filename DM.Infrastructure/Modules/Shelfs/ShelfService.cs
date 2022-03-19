using AutoMapper;
using DM.Core.DTOs.General;
using DM.Core.DTOs.Shelfs;
using DM.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Shelf
{
    public class ShelfService : IShelfService
    {
        private readonly DMDbContext _context;
        private readonly IMapper _mapper;
        public ShelfService(DMDbContext context, IMapper napper)
        {
            _context = context;
            _mapper = napper;
        }

        public async Task Create(CreateShelfDto dto, string userId)
        {
            var Shelf = _mapper.Map<DM.Core.Entities.Shelf>(dto);
            Shelf.CreatedBy = userId;

            if (await _context.Shelfs.AnyAsync(x => !x.IsDelete && x.ShelfNo.Equals(Shelf.ShelfNo) && dto.ExhibitionId == x.ExhibitionId ))
                throw new DMException("Shelf Number Already Exist");

            await _context.Shelfs.AddAsync(Shelf);
            await _context.SaveChangesAsync();
        }



        public async Task Delete(int id, string userId)
        {
            var Shelf = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == id);
            if (Shelf == null)
                throw new DMException("Shelf does't exists");
            if (Shelf.IsDelete)
                throw new DMException("Shelf already deleted");

            if (await _context.ShelfProducts.AnyAsync(x=> !x.IsDelete && x.ShelfId == id && x.Quantity > 0))
                throw new DMException("Can't Delete this shelf because the shelf contains product");


            Shelf.IsDelete = true;
            _context.Shelfs.Update(Shelf);
            await _context.SaveChangesAsync();
        }


        public async Task<ShelfDto> Get(int id)
        {
            var Shelf = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == id);
            if (Shelf == null)
                throw new DMException("Shelf does't exists");
            if (Shelf.IsDelete)
                throw new DMException("Shelf already deleted");
            return _mapper.Map<ShelfDto>(Shelf);
        }


        public async Task<List<ListItemShelfDto>> List(int exhibitionId)
        {
            return await _context.Shelfs
                    .Where(x => !x.IsDelete  && x.ExhibitionId == exhibitionId)
                    .Select(c => new ListItemShelfDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ShelfNo = c.ShelfNo
                    }).OrderBy(x=> Convert.ToInt32(x.ShelfNo)).ToListAsync();
        }


        public async Task Update(UpdateShelfDto dto, string userId)
        {
            var Shelf = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (Shelf == null)
                throw new DMException("Shelf does't exists");
            if (Shelf.IsDelete)
                throw new DMException("Shelf already deleted");

            if (await _context.Shelfs.AnyAsync(x => !x.IsDelete && x.ShelfNo.Equals(Shelf.ShelfNo) && dto.ExhibitionId == x.ExhibitionId && x.Id != dto.Id))
                throw new DMException("Shelf Number Already Exist");

            Shelf.Name = dto.Name;
            Shelf.ShelfNo = dto.ShelfNo.ToString();
            Shelf.ExhibitionId = dto.ExhibitionId;

            _context.Shelfs.Update(Shelf);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShelfDto>> GetAll(GetAllShelfsDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;

            return await _context.Shelfs
                .Where(x => !x.IsDelete && x.ExhibitionId == dto.ExhibitionId 
                && (string.IsNullOrEmpty(dto.SearchKey)
                || x.Name.Contains(dto.SearchKey)))
                .OrderBy(x => Convert.ToInt32(x.ShelfNo)).Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ShelfDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ShelfNo = c.ShelfNo
                }).ToListAsync();
        }
    }
}
