using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
   public class Exhibition : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ExhibitionType Type { get; set; }
        public List<Shelf> Shelfs { get; set; }
    }
}
