using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.General
{
    public class ListItemDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class ListItemShelfDto
    {
        public string Name { get; set; }
        public string ShelfNo { get; set; }
        public int Id { get; set; }
    }
}
