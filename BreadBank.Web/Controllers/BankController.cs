using Microsoft.AspNetCore.Mvc;

namespace BreadBank.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BankController : ControllerBase
	{
		private readonly Manager _manager;
		private readonly Repository _repository;

		public BankController(Manager manager, Repository repository)
		{
			_manager = manager;
			_repository = repository;
		}

		[HttpPost("register")]
		public IActionResult Register([FromQuery] string mail, [FromQuery] string pin)
		{
			try
			{
				var account = _manager.Registration(mail, pin);
				return Ok(account);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("balance")]
		public IActionResult GetBalance([FromQuery] string mail, [FromQuery] string pin)
		{
			try
			{
				var acc = _repository.GetByMail(mail);
				if (acc == null)
					return NotFound("Аккаунт не найден.");
				var balance = _manager.CheckBalance(acc, pin);
				return Ok(new { CurrentBalance = balance });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost("deposit")]
		public IActionResult Deposit([FromQuery] string mail, [FromQuery] string pin, [FromQuery] int amount)
		{
			try
			{
				var acc = _repository.GetByMail(mail);
				if (acc == null)
					return NotFound("Аккаунт не найден");
				_manager.Deposit(acc, amount, pin);
				return Ok($"Счет пополнен. Новый баланс: {acc.balance}");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("withdraw")]
		public IActionResult Withdraw([FromQuery] string mail, [FromQuery] string pin, [FromQuery] int amount)
		{
			try
			{
				var acc = _repository.GetByMail(mail);
				if (acc == null)
					return NotFound("Аккаунт не найден");
				_manager.Withdraw(acc, amount, pin);
				return Ok($"Деньги сняты. Новый баланс: {acc.balance}");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("unban")]
		public IActionResult Unban([FromQuery] string mail, [FromQuery] string pin)
		{
			try
			{
				var acc = _repository.GetByMail(mail);
				if (acc == null)
					return NotFound("Аккаунт не найден");
				_manager.Unban(acc, pin);
				return Ok("Аккаунт успешно разблокирован");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("transfer")]
		public IActionResult Transfer([FromQuery] string mail, [FromQuery] string targetMail, [FromQuery] string pin, [FromQuery] int amount)
		{
			try
			{
				var sender = _repository.GetByMail(mail);
				if (sender == null)
					return NotFound("Аккаунт не найден.");
				var receiver = _repository.GetByMail(targetMail);
				if (receiver == null)
					return NotFound("Аккаунт не найден.");
				_manager.Transfer(receiver, sender, pin, amount);
				return Ok($"Вы успешно перевели {amount} на аккаунт {targetMail}");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	} 
}
