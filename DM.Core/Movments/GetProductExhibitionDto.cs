using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Movments
{
   public class GetProductExhibitionDto : PagingDto
    {
        public int? ExhibitionId { get; set; }
        public int? ShelfId { get; set; }
    }

    public class GetProductStoreDto : PagingDto
    {
    }

    public class GetProductDamagedDto : PagingDto
    {
    }
    public class GetProductFinishedDto : PagingDto
    {
    }
}
