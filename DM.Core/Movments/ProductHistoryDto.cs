using DM.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Movments
{
   public class ProductHistoryDto
    {
        public string FullName { get; set; }
        public DateTime? ActionDate { get; set; }
        public string FromShelf { get; set; }
        public string ToShelf { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int DeferanceQunatity { get; set; }
        public string MovmentType { get; set; }
        public MovmentType MovmentTypeEnum { get; set; }

    }
    public class ProductHistoryReportDto
    {
        public string FullName { get; set; }
        public string ActionDate { get; set; }
        public string FromShelf { get; set; }
        public string ToShelf { get; set; }
        public int OldQuantity { get; set; }
        public int NewQantity { get; set; }
        public int Deferance { get; set; }
        public string MovmantType { get; set; }
        public MovmentType MovmentTypeEnum { get; set; }

    }
}
