using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface IBorrowerRepository : IRepository<Borrower>
    {
        Task<IEnumerable<Borrower>> GetBorrowersWithLoansAsync();
        Task<Borrower?> GetBorrowerByEmailAsync(string email);
    }
}