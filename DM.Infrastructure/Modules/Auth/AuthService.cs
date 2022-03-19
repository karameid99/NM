using AutoMapper;
using DM.Core.DTOs.Auth;
using DM.Core.DTOs.General;
using DM.Core.Entities.Auth;
using DM.Core.Exceptions;
using DM.Infrastructure.Modules.Image;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NM.Data.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DMDbContext _context;
        private readonly UserManager<DMUser> _userService;
        private readonly SignInManager<DMUser> _signInManager;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(DMDbContext context, UserManager<DMUser> userService, SignInManager<DMUser> signInManager, IImageService imageService, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _signInManager = signInManager;
            _imageService = imageService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Create(CreateUserDto dto)
        {
            var user = _mapper.Map<DMUser>(dto);
            if (dto.Image != null) user.ImagePath = await _imageService.Save(dto.Image, "Images");
            user.UserType = Core.Enums.UserType.Supervisor;
            user.Email = dto.UserName;
            var result = await _userService.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new DMException("UserName Already Used");
        }

        public async Task Delete(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new DMException("User does't exists");
            if (user.IsDelete)
                throw new DMException("User already deleted");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDto> Get(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new DMException("User does't exists");
            if (user.IsDelete)
                throw new DMException("User already deleted");
            return _mapper.Map<UserDto>(user);

        }

        public async Task<List<UserDto>> GetAll(PagingDto dto)
        {
            var skipValue = (dto.Page - 1) * dto.PerPage;
            return await _context.Users
                .Where(x => !x.IsDelete && x.UserType == Core.Enums.UserType.Supervisor
                && (string.IsNullOrEmpty(dto.SearchKey)
                || x.FirstName.Contains(dto.SearchKey)
                || x.LastName.Contains(dto.SearchKey)))
                .Skip(skipValue).Take(dto.PerPage)
                .Select(c => new UserDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.UserName,
                    ImagePath = c.ImagePath
                }).ToListAsync();
        }

        public async Task<UserLoginDto> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
            if (user == null)
                throw new DMException("Invalid username");
            if (user.IsDelete)
                throw new DMException("Your account is deleted");
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                throw new DMException("Invalid password");

            string token = CreateAccess(user);
            return new UserLoginDto
            {
                ImagePath = user.ImagePath,
                Token = token,
                UserName = user.UserName,
                UserType = user.UserType
            };
        }

        public async Task ChangeAdminPassword(ChangePasswordAdmindto dto, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new DMException("User does't exists");
            if (user.IsDelete)
                throw new DMException("User already deleted");

            var token = await _userService.GeneratePasswordResetTokenAsync(user);
            await _userService.ResetPasswordAsync(user, token, dto.Password);

        }
        public async Task Update(UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (user == null)
                throw new DMException("User does't exists");
            if (user.IsDelete)
                throw new DMException("User already deleted");
            if (await _context.Users.AnyAsync(x => x.UserName == dto.UserName && x.Id != dto.Id))
                throw new DMException("UserName already used");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UserName = dto.UserName;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userService.GeneratePasswordResetTokenAsync(user);
                await _userService.ResetPasswordAsync(user, token, dto.Password);
            }
           
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

        }
        private string CreateAccess(DMUser user)
        {
            var userType = (user.UserType).ToString();
            var claims = new List<Claim>(){
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.Id),
                new Claim("UserType",userType ),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var token = new JwtSecurityTokenHandler().WriteToken(accessToken);
            return token;
        }
    }
}
