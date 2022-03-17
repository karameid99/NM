using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Products
{
   public class SearchProductDto
    {
        public int Id { get; set; }
        public int ShelfId { get; set; }
        public int ExhibitionId { get; set; }
        public string ProductNo { get; set; }
        public string ShelfNo { get; set; }
        public string ProductName { get; set; }
        public string ShelfName { get; set; }
        public string ExhibitionName { get; set; }
        public string LogoPath { get; set; }
        public int Quantity { get; set; }
        public bool IsStore { get; set; }
    }
}
