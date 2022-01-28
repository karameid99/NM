using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Auth
{
   public class UpdateUserDto : CreateUserDto
    {
        public string Id { get; set; }
    }
}
