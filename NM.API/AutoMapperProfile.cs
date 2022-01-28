using AutoMapper;
using DM.Core.DTOs.Auth;
using DM.Core.DTOs.Exhibitions;
using DM.Core.DTOs.Products;
using DM.Core.DTOs.Shelfs;
using DM.Core.Entities;
using DM.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DMUser, UserDto>();
            CreateMap<CreateUserDto, DMUser>();

            CreateMap<Product, CreateProductDto>();
            CreateMap<Shelf, CreateShelfDto>();
            CreateMap<Exhibition, CreateExhibitionDto>();
        }
    }
}
