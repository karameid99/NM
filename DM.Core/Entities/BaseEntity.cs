﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Entities
{
   public class BaseEntity
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; }
    
    }
}
