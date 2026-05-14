using BreadBank.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace BreadBank.Web.Repository
{
	public class Repository
	{
		public async Task Add(Account account) 
		{
			using (var db = new AppDbContext())
			{
				await db.Accounts.AddAsync(account);
				await db.SaveChangesAsync();
			}
		}
		public async Task<Account?> GetByMail(string mail)
		{
			using (var db = new AppDbContext())
			{
				return await db.Accounts.FirstOrDefaultAsync(p => p.Mail == mail);
			}
		}

		public async Task Update(Account account)
		{
			using (var db = new AppDbContext())
			{
				db.Accounts.Update(account);
				await db.SaveChangesAsync();
			}
		}

		public async Task AddTransaction(Transaction transaction)
		{
			using (var db = new AppDbContext())
			{
				await db.Transactions.AddAsync(transaction);
				await db.SaveChangesAsync();
			}
		}

		public async Task<List<Transaction>> GetHistory(int id)
		{
			using (var db = new AppDbContext())
			{
				return await 
					db.Transactions
					.Where(t => t.AccountId == id)
					.OrderByDescending(t => t.CreatedAt)
					.Take(10)
					.ToListAsync();
			}
		}
	}
}
