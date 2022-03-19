using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Reports
{
   public class SearchProductDto
    {
        public int Quantity { get; set; }
        public string ShelfNo { get; set; }
        public string ProductNo { get; set; }
    }
}
