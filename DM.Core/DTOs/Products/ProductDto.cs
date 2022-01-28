using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Products
{
   public class ProductDto : BaseDto
    {
        public string ProductNo { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }
    }
}
