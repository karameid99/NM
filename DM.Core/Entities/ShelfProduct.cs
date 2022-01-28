using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
    public class ShelfProduct : BaseEntity
    {
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int ShelfId { get; set; }
        [ForeignKey(nameof(ShelfId))]
        public Shelf Shelf { get; set; }
        public int Quantity { get; set; }
        public List<ProductHistory> NewProductHistories { get; set; }
        public List<ProductHistory> OldProductHistories { get; set; }
    }
}
