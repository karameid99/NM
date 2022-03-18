using AutoMapper;
using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.General;
using DM.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using NM.Data.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DM.Core.Entities;
using DM.Core.Enums;

namespace DM.Infrastructure.Modules.Exhibition
{
    public class ExhibitionService : IExhibitionService
    {
        private readonly DMDbContext _context;
        private readonly IMapper _mapper;
        public ExhibitionService(DMDbContext context, IMapper napper)
        {
            _context = context;
            _mapper = napper;
        }

        public async Task Create(CreateExhibitionDto dto, string userId, ExhibitionType type)
        {
            var exhibition = _mapper.Map<DM.Core.Entities.Exhibition>(dto);
            exhibition.CreatedBy = userId;
            exhibition.Type = type;
            await _context.Exhibitions.AddAsync(exhibition);
            await _context.SaveChangesAsync();
        }



        public async Task Delete(int id, string userId, ExhibitionType type)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == id && x.Type == type);

            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");

            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");

            if (await _context.Shelfs.AnyAsync(x => !x.IsDelete && x.ExhibitionId == id))
                throw new DMException($"Can't Delete this {type} because the {type} contains Shelfs");

            exhibition.IsDelete = true;
            _context.Exhibitions.Update(exhibition);
            await _context.SaveChangesAsync();
        }


        public async Task<ExhibitionDto> Get(int id, ExhibitionType type)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == id && x.Type == type);
            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");
            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");
            return _mapper.Map<ExhibitionDto>(exhibition);
        }


        public async Task<List<ListItemDto>> List(ExhibitionType type)
        {
            return await _context.Exhibitions
                    .Where(x => !x.IsDelete && x.Type == type)
                    .Select(c => new ListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                    }).ToListAsync();
        }


        public async Task Update(UpdateExhibitionDto dto, string userId, ExhibitionType type)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == dto.Id && x.Type == type);
            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");
            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");

            exhibition.Name = dto.Name;
            exhibition.Address = dto.Address;
           
            _context.Exhibitions.Update(exhibition);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ExhibitionDto>> GetAll(PagingDto dto, ExhibitionType type)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;

            return await _context.Exhibitions
                .Where(x => !x.IsDelete && x.Type == type
                && (string.IsNullOrEmpty(dto.SearchKey)
                || x.Name.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new ExhibitionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address
                }).ToListAsync();
        }
    }
}
