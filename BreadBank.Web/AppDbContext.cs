using Microsoft.EntityFrameworkCore;

namespace BreadBank.Web
{
	public class AppDbContext:DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bank-db;Username=postgres;Password=postgres");
		}
	}
}
