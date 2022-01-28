using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Exhibitions
{
   public class CreateExhibitionDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class UpdateExhibitionDto : CreateExhibitionDto
    {
        public int Id { get; set; }
    }
}
