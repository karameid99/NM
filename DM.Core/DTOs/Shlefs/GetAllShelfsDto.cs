using DM.Core.DTOs.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Shelfs
{
   public class GetAllShelfsDto : PagingDto
    {
        public int ExhibitionId { get; set; }
    }
}
