using System.ComponentModel.DataAnnotations;

namespace BreadBank.Web
{
	public class Account
	{
		[Key]
		public int Id { get; set; }
		public string Mail { get; set; }
		public string Pincode { get; set; }
		public bool IsBanned { get; set; } = false;
		public int FailedAttempts { get; set; } = 0;

		private int _balance = 1000;
		public int Balance
		{
			get { return _balance; }
			set { _balance = value; }
		}

		public List<Transaction> Transactions { get; set; } = new List<Transaction>();
	}
}
