using Microsoft.AspNetCore.Mvc;
using Bao.Data;
using Bao.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Bao.Controllers.Manager
{
    public class ReportViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly BaoContext _context;

        public ReportViewController(BaoContext context)
        {
            _context = context;
        }

        public IActionResult SalesReport(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            // Ensure end date includes the entire day
            end = end.AddDays(1).AddTicks(-1);

            var reportData = _context.OrderItems
                .Join(_context.Orders, oi => oi.OrderId, o => o.OrderId, (oi, o) => new { oi, o })
                .GroupBy(g => new { g.o.OrderId, g.o.OrderDate })
                .Select(g => new ReportView
                {
                    OrderId = g.Key.OrderId,
                    OrderDate = g.Key.OrderDate,
                    TotalAmount = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice)
                })
                .Where(r => r.OrderDate >= start && r.OrderDate <= end)
                .ToList();

            var totalSales = reportData.Sum(rd => rd.TotalAmount);

            var viewModel = new SalesReportViewModel
            {
                ReportData = reportData,
                TotalSales = totalSales,
                StartDate = start,
                EndDate = end
            };

            return View(viewModel);
        }

        public IActionResult ProductSalesReport(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            // Ensure end date includes the entire day
            end = end.AddDays(1).AddTicks(-1);

            var productSalesData = _context.OrderItems
                .Join(_context.Products, oi => oi.ProductId, p => p.ProductId, (oi, p) => new { oi, p, oi.Order.OrderDate })
                .Where(x => x.OrderDate >= start && x.OrderDate <= end)
                .GroupBy(g => g.p.ProductName)
                .Select(g => new ProductSalesViewModel
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.oi.Quantity),
                    TotalSales = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice)
                })
                .ToList();

            var totalSales = productSalesData.Sum(ps => ps.TotalSales);

            var viewModel = new SalesReportViewModel
            {
                ProductSalesData = productSalesData,
                TotalSales = totalSales,
                StartDate = start,
                EndDate = end
            };

            return View(viewModel);
        }

        public IActionResult ExportSalesReportToPdf(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            // Ensure end date includes the entire day
            end = end.AddDays(1).AddTicks(-1);

            var reportData = _context.OrderItems
                .Join(_context.Orders, oi => oi.OrderId, o => o.OrderId, (oi, o) => new { oi, o })
                .GroupBy(g => new { g.o.OrderId, g.o.OrderDate })
                .Select(g => new ReportView
                {
                    OrderId = g.Key.OrderId,
                    OrderDate = g.Key.OrderDate,
                    TotalAmount = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice)
                })
                .Where(r => r.OrderDate >= start && r.OrderDate <= end)
                .ToList();

            var totalSales = reportData.Sum(rd => rd.TotalAmount);

            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph("Sales Report"));
                pdfDoc.Add(new Paragraph("Date: " + DateTime.Now.ToString("yyyy-MM-dd")));
                pdfDoc.Add(new Paragraph($"Period: {start.ToString("yyyy-MM-dd")} to {end.ToString("yyyy-MM-dd")}"));
                pdfDoc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 20f, 20f });

                table.AddCell("Order ID");
                table.AddCell("Order Date");
                table.AddCell("Total Amount");

                foreach (var data in reportData)
                {
                    table.AddCell(data.OrderId.ToString());
                    table.AddCell(data.OrderDate.ToString("yyyy-MM-dd"));
                    table.AddCell(data.TotalAmount.ToString("C"));
                }

                // Add the total sales row
                PdfPCell totalCell = new PdfPCell(new Phrase("Total Sales"));
                totalCell.Colspan = 2;
                totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(totalCell);

                table.AddCell(totalSales.ToString("C"));

                pdfDoc.Add(table);
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "Bao_SalesReport.pdf");
            }
        }


        public IActionResult ExportProductSalesReportToPdf(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            // Ensure end date includes the entire day
            end = end.AddDays(1).AddTicks(-1);

            var productSalesData = _context.OrderItems
                .Join(_context.Products, oi => oi.ProductId, p => p.ProductId, (oi, p) => new { oi, p, oi.Order.OrderDate })
                .Where(x => x.OrderDate >= start && x.OrderDate <= end)
                .GroupBy(g => g.p.ProductName)
                .Select(g => new ProductSalesViewModel
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.oi.Quantity),
                    TotalSales = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice)
                })
                .ToList();

            var totalSales = productSalesData.Sum(ps => ps.TotalSales);

            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph("Product Sales Report"));
                pdfDoc.Add(new Paragraph("Date: " + DateTime.Now.ToString("yyyy-MM-dd")));
                pdfDoc.Add(new Paragraph($"Period: {start.ToString("yyyy-MM-dd")} to {end.ToString("yyyy-MM-dd")}"));
                pdfDoc.Add(new Paragraph("Total Sales: $" + totalSales));
                pdfDoc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 40f, 30f, 30f });

                table.AddCell("Product Name");
                table.AddCell("Total Quantity");
                table.AddCell("Total Sales");

                foreach (var data in productSalesData)
                {
                    table.AddCell(data.ProductName);
                    table.AddCell(data.TotalQuantity.ToString());
                    table.AddCell(data.TotalSales.ToString("C"));
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "ProductSalesReport.pdf");
            }
        }
    }
}
