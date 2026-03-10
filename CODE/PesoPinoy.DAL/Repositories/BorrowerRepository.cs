using Microsoft.EntityFrameworkCore;
using PesoPinoy.DAL.Data;
using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public class BorrowerRepository : Repository<Borrower>, IBorrowerRepository
    {
        public BorrowerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Borrower>> GetBorrowersWithLoansAsync()
        {
            return await _context.Borrowers
                .Include(b => b.Loans)
                .ToListAsync();
        }

        public async Task<Borrower?> GetBorrowerByEmailAsync(string email)
        {
            return await _context.Borrowers
                .FirstOrDefaultAsync(b => b.Email == email);
        }
    }
}