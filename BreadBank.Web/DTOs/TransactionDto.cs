namespace BreadBank.Web.DTOs
{
	public class TransactionDto
	{
		public string Type { get; set; }
		public int Amount { get; set; }
		public string TargetMail { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
