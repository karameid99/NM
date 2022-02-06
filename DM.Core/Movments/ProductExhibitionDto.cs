using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Movments
{
   public class ProductExhibitionDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShelfName { get; set; }
        public string LogoPath { get; set; }
        public int ShelfId { get; set; }
        public int Quantity  { get; set; }
    }
}
