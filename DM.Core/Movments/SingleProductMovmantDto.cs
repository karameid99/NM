using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Movments
{
    public class SingleProductMovmantDto
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int ShelfId { get; set; }
        public int Quantity { get; set; }
        public MovmentActionType MovmentActionType { get; set; }
    }
    public class MultipleProductMovmantDto
    {
        public MovmentActionType MovmentActionType { get; set; }
        public List<MultipleProductMovmantItemDto> Items { get; set; }
    }
    public class MultipleProductMovmantItemDto
    {
        public int ProductId { get; set; }
        public int? ShelfId { get; set; }
        public int Quantity { get; set; }
    }
   
}
