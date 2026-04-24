using System.ComponentModel.DataAnnotations;

namespace BreadBank.Web
{
	public class Account
	{
		[Key]
		public int id { get; set; }
		public string mail { get; set; }
		public int pincode { get; set; }
		
		public bool isBanned { get; set; } = false;
		public int failedAttempts { get; set; } = 0;

		private int _balance = 1000;
		public int balance
		{
			get { return _balance; }
			set { _balance = value; }
		}
	}
}
