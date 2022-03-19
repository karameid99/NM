using DM.Infrastructure.Modules.Movments.Reports.ReportsModule;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NM.API.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> SearchProduct(string searchKey)
        {
            return File(await _reportService.SearchProduct(searchKey), "Application/pdf");
        }
        public async Task<IActionResult> WearhouesProducts(int exhibitionId)
        {
            return File(await _reportService.WearhouesProducts(exhibitionId), "Application/pdf");
        }
        public async Task<IActionResult> Products(string searchKey)
        {
            return File(await _reportService.Products(searchKey), "Application/pdf");
        }
        public async Task<IActionResult> ShelfProducts(int shelfId)
        {
            return File(await _reportService.ShelfProducts(shelfId), "Application/pdf");
        }
    }
}
