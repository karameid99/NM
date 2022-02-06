using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Products
{
   public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ProductNo { get; set; }
        public string Description { get; set; }
        public IFormFile Logo { get; set; }
    }
    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
    }
}
