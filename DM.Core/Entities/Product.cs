using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
   public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string ProductNo { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }
    }
}
