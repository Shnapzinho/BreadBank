using System.ComponentModel.DataAnnotations;
namespace BreadBank.Web.Entity
{
	public class Transaction
	{
		[Key]
		public int Id { get; set; }

		public int AccountId { get; set; }

		public string Type { get; set; }
		public int Amount { get; set; }
		public string? TargetMail { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;

	}
}
