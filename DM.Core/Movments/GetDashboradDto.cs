using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Movments
{
   public class GetDashboradDto
    {
        public int CurrentProducts { get; set; }
        public int FinishedProducts { get; set; }
        public int DamagedProducts { get; set; }
        public int StoreProducts { get; set; }
        public int Exhibitions { get; set; }
        public int Users { get; set; }
    }
}
