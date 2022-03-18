using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Auth
{
   public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        public IFormFile Image { get; set; }
    }
}
