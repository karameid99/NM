using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.General
{
    public class PagingDto
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; }
        [Range(1, 50)]
        public int PerPage { get; set; }
        public string SearchKey { get; set; }
    }
}
