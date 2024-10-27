using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<MonthlyReportDto>> GenerateMonthlyReportAsync(int month, int year)
        {
            var orders = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .ToListAsync();

            var report = orders.Select(o => new MonthlyReportDto
            {
                UserFullName = o.User.FullName,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new ReportOrderItemDto
                {
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });

            return report;
        }

        public async Task<byte[]> GenerateReportByDateRangeExcelAsync(DateTime fromDate, DateTime toDate)
        {
            var orders = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                // Add headers
                worksheet.Cells[1, 1].Value = "User";
                worksheet.Cells[1, 2].Value = "Order Date";
                worksheet.Cells[1, 3].Value = "Total Amount";
                worksheet.Cells[1, 4].Value = "Book Title";
                worksheet.Cells[1, 5].Value = "Quantity";
                worksheet.Cells[1, 6].Value = "Unit Price";
                worksheet.Cells[1, 7].Value = "Total Price";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        worksheet.Cells[row, 1].Value = order.User.FullName;
                        worksheet.Cells[row, 2].Value = order.OrderDate.ToShortDateString();
                        worksheet.Cells[row, 3].Value = order.TotalAmount;
                        worksheet.Cells[row, 4].Value = item.Book.Title;
                        worksheet.Cells[row, 5].Value = item.Quantity;
                        worksheet.Cells[row, 6].Value = item.UnitPrice;
                        worksheet.Cells[row, 7].Value = item.Quantity * item.UnitPrice;

                        row++;
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

    }
}
