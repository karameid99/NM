using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.Shelfs
{
   public class CreateShelfDto
    {
        public string Name { get; set; }
        public string ShelfNo { get; set; }
        public int ExhibitionId { get; set; }
    }
    public class UpdateShelfDto : CreateShelfDto
    {
        public int Id { get; set; }
    }
}
