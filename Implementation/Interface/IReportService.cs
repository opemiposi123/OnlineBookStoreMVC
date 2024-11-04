using OnlineBookStoreMVC.DTOs;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IReportService
    {
        Task<IEnumerable<MonthlyReportDto>> GenerateMonthlyReportAsync(int month, int year);
        Task<byte[]> GenerateReportByDateRangeExcelAsync(DateTime fromDate, DateTime toDate);
    }
}
