using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Shelfs
{
   public class CreateShelfDto
    {
        [Required]
        public string Name { get; set; }
        public string ShelfNo { get; set; }
        public int ExhibitionId { get; set; }
    }
    public class UpdateShelfDto : CreateShelfDto
    {
        public int Id { get; set; }
    }
}
