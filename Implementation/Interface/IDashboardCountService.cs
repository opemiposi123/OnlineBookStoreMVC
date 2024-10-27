using OnlineBookStoreMVC.DTOs;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IDashboardCountService
    {
        Task<DashboardCountDto> DashBoardCount();
    }
}
