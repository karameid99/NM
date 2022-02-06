using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Exhibitions
{
   public class CreateExhibitionDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
    public class UpdateExhibitionDto : CreateExhibitionDto
    {
        public int Id { get; set; }
    }
}
