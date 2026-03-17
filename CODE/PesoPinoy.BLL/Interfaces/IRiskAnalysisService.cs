using System.Collections.Generic;
using System.Threading.Tasks;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.BLL.Interfaces
{
    public interface IRiskAnalysisService
    {
        Task<Dictionary<string, object>> GetPortfolioRiskAnalysisAsync();
        Task<List<Borrower>> GetBorrowersNeedingReviewAsync();
        Task<decimal> CalculateRiskScore(Borrower borrower);
        Task UpdateRiskScoresAsync();
        Task<Dictionary<string, int>> GetRiskDistributionAsync();
        Task<decimal> GetHighRiskExposureAsync();
    }
}
