﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DM.Data;
using DM.Core.Entities.Auth;
using DM.Core.Enums;
using NM.Data.Data;

namespace DM.Data
{
    public static class DbSeeder
    {
        public static IHost SeedDb(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    var DMDbContext = scope.ServiceProvider.GetRequiredService<DMDbContext>(); 
                    var _userManager  = scope.ServiceProvider.GetService<UserManager<DMUser>>();
                    DMDbContext.SeedAdmin(_userManager).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
            return webHost;
        }
       
        public static async Task SeedAdmin(this DMDbContext userManager, UserManager<DMUser> _userManager)
        {
            if (await userManager.Users.AnyAsync(x=> x.UserType == UserType.Admin))
                return;
            var user = new DMUser()
            {
                UserName = "AboYusef",
                UserType = UserType.Admin,
                FirstName = "Abo",
                LastName = "Yusef",
            };
            await _userManager.CreateAsync(user , "Admin11$");
            await userManager.SaveChangesAsync();
        }
    }
}
