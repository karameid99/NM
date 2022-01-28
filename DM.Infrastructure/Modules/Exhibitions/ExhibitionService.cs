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

        public async Task Create(CreateExhibitionDto dto, string userId)
        {
            var exhibition = _mapper.Map<DM.Core.Entities.Exhibition>(dto);
            exhibition.CreatedBy = userId;
            exhibition.Type = Core.Enums.ExhibitionType.Exhibition;
            await _context.Exhibitions.AddAsync(exhibition);
            await _context.SaveChangesAsync();
        }



        public async Task Delete(int id, string userId)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == id);
            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");
            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");

            exhibition.IsDelete = true;
            _context.Exhibitions.Update(exhibition);
            await _context.SaveChangesAsync();
        }


        public async Task<ExhibitionDto> Get(int id)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == id);
            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");
            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");
            return _mapper.Map<ExhibitionDto>(exhibition);
        }


        public async Task<List<ListItemDto>> List()
        {
            return await _context.Exhibitions
                    .Where(x => !x.IsDelete && x.Type == Core.Enums.ExhibitionType.Exhibition)
                    .Select(c => new ListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                    }).ToListAsync();
        }


        public async Task Update(UpdateExhibitionDto dto, string userId)
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (exhibition == null)
                throw new DMException("Exhibitions does't exists");
            if (exhibition.IsDelete)
                throw new DMException("Exhibitions already deleted");

            exhibition.Name = dto.Name;
            exhibition.Address = dto.Address;
           
            _context.Exhibitions.Update(exhibition);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ExhibitionDto>> GetAll(PagingDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;

            return await _context.Exhibitions
                .Where(x => !x.IsDelete && x.Type == Core.Enums.ExhibitionType.Exhibition
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
