using BreadBank.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace BreadBank.Web.Config
{
	public class AppDbContext:DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
			if (string.IsNullOrEmpty(connString))
				connString = "Host=localhost;Port=5432;Database=bank_db;Username=postgres;Password=postgres";
			optionsBuilder.UseNpgsql(connString);
		}
	}
}
