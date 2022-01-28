using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Auth
{
    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public UserType UserType { get; set; }
        public string Token { get; set; }
    }
}
