using DM.Core.DTOs.Reports;
using DM.Core.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using NM.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments.Reports.ReportsModule
{
    public class ReportService : IReportService
    {
        private const string slash = "\\";
        private readonly DMDbContext _context;
        private readonly IWebHostEnvironment environment;

        public ReportService(DMDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

        public async Task<byte[]> Products(string searchKey)
        {
            var products = await _context.Products
                  .Where(x => !x.IsDelete
                  && (string.IsNullOrEmpty(searchKey)
                  || x.ProductNo.Contains(searchKey)))
                  .Select(c => new ProductsDto
                  {
                      Name = c.Name ?? "---",
                      Descrption = c.Description ?? "---",
                      Number = c.ProductNo ?? "---"
                  }).ToListAsync();

            List<ReportParameter> parameters = new List<ReportParameter>();
            return Print(parameters, "Products.rdlc", "Products", products);
        }
        public async Task<byte[]> SearchProduct(string searchKey)
        {
            var products = await _context.ShelfProducts.Where(x =>
           !x.IsDelete &&
           x.Shelf.Exhibition.Type != ExhibitionType.Store
           && (string.IsNullOrEmpty(searchKey)
           || x.Product.Name.Contains(searchKey)
           || x.Product.ProductNo.Contains(searchKey)
           || x.Shelf.Name.Contains(searchKey)
           || x.Shelf.ShelfNo.Contains(searchKey)
           || x.Shelf.Exhibition.Name.Contains(searchKey)
           )).OrderByDescending(x => x.Id)
           .Select(x => new SearchProductDto
           {
               ProductNo = x.Product.ProductNo,
               Quantity = x.Quantity,
               ShelfNo = x.Shelf.ShelfNo,
           }).ToListAsync();

            List<ReportParameter> parameters = new List<ReportParameter>();
            return Print(parameters, "SearchProduct.rdlc", "SearchProducts", products);
        }
        public async Task<byte[]> ShelfProducts(int shelfId)
        {
            var shelfNo = await _context.Shelfs.FirstOrDefaultAsync(x => x.Id == shelfId);
            var products = await _context.ShelfProducts
                            .Where(x => !x.IsDelete && x.ShelfId == shelfId)
                            .Select(c => new SearchProductDto
                            {
                                Quantity = c.Quantity,
                                ProductNo = c.Product.ProductNo,
                                ShelfNo = c.Shelf.ShelfNo
                            }).ToListAsync();

            List<ReportParameter> parameters = new List<ReportParameter>() { new ReportParameter("ShelfNo", shelfNo.ShelfNo) };
            return Print(parameters, "ShelfProduct.rdlc", "ShelfProduct", products);
        }
        public async Task<byte[]> WearhouesProducts(int exhibitionId)
        {
            var wearhouse = await _context.Exhibitions.FirstOrDefaultAsync(x => x.Id == exhibitionId);
            var products = await _context.ShelfProducts.Where(x => !x.IsDelete && x.Shelf.ExhibitionId == exhibitionId)
                                        .Select(c => new SearchProductDto
                                        {
                                            Quantity = c.Quantity,
                                            ProductNo = c.Product.ProductNo,
                                            ShelfNo = c.Shelf.ShelfNo
                                        }).ToListAsync();

            List<ReportParameter> parameters = new List<ReportParameter>() { new ReportParameter("WearhouseName", wearhouse.Name ?? "---"), new ReportParameter("WearhouseAddress", wearhouse.Address ?? "---") };
            return Print(parameters, "WearhouseProduct.rdlc", "ShelfProduct", products);
        }
        private byte[] Print(List<ReportParameter> parameters, string reportName, string dataSourcsName, object dataSourc)
        {
            var reportPath = environment.WebRootPath + slash + reportName;
            LocalReport report = new LocalReport();
            report.ReportPath = reportPath;
            report.SetParameters(parameters);
            report.DataSources.Add(new ReportDataSource(dataSourcsName, dataSourc));
            return report.Render("PDF");

        }
    }
}
