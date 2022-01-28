using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
    public class Shelf : BaseEntity
    {
        public string Name { get; set; }
        public string ShelfNo { get; set; }
        public int ExhibitionId { get; set; }
        [ForeignKey(nameof(ExhibitionId))]
        public Exhibition Exhibition { get; set; }
    }
}
