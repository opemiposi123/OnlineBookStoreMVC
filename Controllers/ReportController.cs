using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Implementation.Services;

namespace OnlineBookStoreMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        public async Task<IActionResult> MonthlyReport(int month, int year)
        {
            var report = await _reportService.GenerateMonthlyReportAsync(month, year); 
            return View(report);
        }

        [HttpGet]
        [ActionName("GenerateReport")] 
        public IActionResult DisplayReportForm()
        {
            return View(); 
        }

        [HttpPost]
        [ActionName("GenerateReport")] 
        public async Task<IActionResult> GenerateReportByDateRange(DateTime fromDate, DateTime toDate)
        {
            var excelData = await _reportService.GenerateReportByDateRangeExcelAsync(fromDate, toDate);

            var fileName = $"Report_{fromDate.ToShortDateString()}_to_{toDate.ToShortDateString()}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
