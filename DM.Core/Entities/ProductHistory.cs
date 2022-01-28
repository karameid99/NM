using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
   public class ProductHistory : BaseEntity
    {
        public int NewShelfProductId { get; set; }
        [ForeignKey(nameof(NewShelfProductId))]
        public ShelfProduct NewShelfProduct { get; set; }
        public int? OldShelfProductId { get; set; }
        [ForeignKey(nameof(OldShelfProductId))]
        public ShelfProduct OldShelfProduct { get; set; }
        public string UserFullName { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int AddedQuantity { get; set; }
        public MovmentType MovmentType { get; set; }
    }
}
