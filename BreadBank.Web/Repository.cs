namespace BreadBank.Web
{
	public class Repository
	{
		public void Add(Account account) 
		{
			using (var db = new AppDbContext())
			{
				db.Accounts.Add(account);
				db.SaveChanges();
			}
		}
		public Account GetByMail(string mail)
		{
			using (var db = new AppDbContext())
			{
				return db.Accounts.FirstOrDefault(p => p.mail == mail);
			}
		}

		public void Update(Account account)
		{
			using (var db = new AppDbContext())
			{
				db.Accounts.Update(account);
				db.SaveChanges();
			}
		}
	}
}
