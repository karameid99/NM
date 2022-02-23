using AutoMapper;
using DM.Core.DTOs.General;
using DM.Core.DTOs.Shelfs;
using DM.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
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


        public async Task<List<ListItemDto>> List(int exhibitionId)
        {
            return await _context.Shelfs
                    .Where(x => !x.IsDelete && x.Exhibition.Type == Core.Enums.ExhibitionType.Exhibition && x.ExhibitionId == exhibitionId)
                    .Select(c => new ListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                    }).ToListAsync();
        }


        public async Task Update(UpdateShelfDto dto, string userId)
        {
            var Shelf = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (Shelf == null)
                throw new DMException("Shelf does't exists");
            if (Shelf.IsDelete)
                throw new DMException("Shelf already deleted");
             
            Shelf.Name = dto.Name;
            Shelf.ShelfNo = dto.ShelfNo;
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
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ShelfDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ShelfNo = c.ShelfNo
                }).ToListAsync();
        }
    }
}
