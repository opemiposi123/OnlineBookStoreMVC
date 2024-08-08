using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto> GetReviewByIdAsync(Guid id);
        Task<ReviewDto> CreateReviewAsync(ReviewRequestModel reviewRequest);
        Task<ReviewDto> UpdateReviewAsync(Guid id, ReviewRequestModel reviewRequest);
        Task<bool> DeleteReviewAsync(Guid id);
    }
}
