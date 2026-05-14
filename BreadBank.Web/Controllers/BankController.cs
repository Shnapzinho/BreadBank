using BreadBank.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BreadBank.Web.Repositories;
using BreadBank.Web.DTOs;

namespace BreadBank.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BankController : ControllerBase
	{
		private readonly Manager _manager;


		public BankController(Manager manager, Repository repository)
		{
			_manager = manager;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromQuery] string mail, [FromQuery] string pin)
		{
			var accountDto = await _manager.RegistrationAsync(mail, pin);
			return Ok(accountDto);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromQuery] string mail, [FromQuery] string pin)
		{
			var token = await _manager.LoginAsync(mail, pin);
			return Ok(new { Token = token });
		}

		[HttpGet("balance")]
		[Authorize]
		public async Task<IActionResult> GetBalance()
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			var balance = await _manager.CheckBalanceAsync(mail);
			return Ok(new { CurrentBalance = balance });
		}
		[HttpPost("deposit")]
		[Authorize]
		public async Task<IActionResult> Deposit([FromQuery] int amount)
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			await _manager.DepositAsync(mail, amount);
			return Ok($"Счет пополнен.");
		}

		[HttpPost("withdraw")]
		[Authorize]
		public async Task<IActionResult> Withdraw([FromQuery] int amount)
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			await _manager.WithdrawAsync(mail, amount);
			return Ok($"Деньги сняты.");
		}

		[HttpPost("unban")]
		public async Task<IActionResult> Unban([FromQuery] string mail, [FromQuery] string pin)
		{
			await _manager.UnbanAsync(mail, pin);
			return Ok("Аккаунт успешно разблокирован");
		}

		[HttpPost("transfer")]
		[Authorize]
		public async Task<IActionResult> Transfer([FromQuery] string targetMail, [FromQuery] int amount)
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			await _manager.TransferAsync(targetMail, mail, amount);
			return Ok($"Вы успешно перевели {amount} на аккаунт {targetMail}");
		}

		[HttpGet("history")]
		public async Task <IActionResult> getHistory()
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			var historyDto = await _manager.GetHistoryAsync(mail);
			return Ok(historyDto);
		}

		[HttpPost("change-pin")]
		public async Task<IActionResult> changePin([FromBody] ChangePinRequest request)
		{
			var mail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(mail))
				return Unauthorized();
			await _manager.ChangePincode(mail, request.newPin);
			return Ok(new {message = "Вы успешно сменили пинкод."});
		}

	}
}
