using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments.Reports.ReportsModule
{
    public interface IReportService
    {
        Task<byte[]> SearchProduct(string searchKey);
        Task<byte[]> WearhouesProducts(int exhibitionId);
        Task<byte[]> Products(string searchKey);
        Task<byte[]> ShelfProducts(int shelfId);
    }
}
