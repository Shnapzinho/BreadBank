using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BreadBank.Web.DTOs;
using BCrypt.Net;
using BreadBank.Web.Entity;
using BreadBank.Web.Repositories;

namespace BreadBank.Web.Service
{
	public class Manager
	{
		private Repository _repository;
		private readonly IConfiguration _config;
		public Manager(Repository repository, IConfiguration config) 
		{
			_repository = repository;
			_config = config;
		}

		public async Task<string> LoginAsync (string mail, string pin)
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден.");
			await CheckPinAsync(acc, pin, false);
			return GenerateJwtToken(acc);
		}

		public string GenerateJwtToken(Account account)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[] {
				new Claim(ClaimTypes.Email, account.Mail),
				new Claim("AccountId", account.Id.ToString())
			};

			var token = new JwtSecurityToken(
				_config["Jwt:Issuer"],
				_config["Jwt:Audience"],
				claims,
				expires: DateTime.Now.AddMinutes(60),
				signingCredentials: credentials); 
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<int> CheckBalanceAsync(string mail) 
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден");
			return acc.Balance;
		}
		public async Task DepositAsync(string mail, int input)
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден");
			if (input < 0)
				throw new Exception("Введена некорректная сумма пополнения.");
			acc.Balance += input;
			await _repository.AddTransaction(new Transaction { AccountId = acc.Id, Type = "Пополнение", Amount = input });
			await _repository.Update(acc);
		}
		public async Task WithdrawAsync(string mail, int input)
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден");
			if (input <= 0)
				throw new Exception("Cумма снятия денежных средств должна быть больше 0.");
			if (acc.Balance < input)
				throw new Exception("Недостаточно денежных средств.");
			acc.Balance -= input;
			await _repository.AddTransaction(new Transaction {AccountId = acc.Id, Type = "Снятие", Amount = -input});
			await _repository.Update(acc);
		}
		public async Task<AccountDto> RegistrationAsync(string mail, string pincode) 
		{
			if (await _repository.GetByMail(mail) != null)
				throw new Exception("Аккаунт с такой почтой уже существует.");
			if (pincode.Length != 4)
				throw new Exception("Неверный формат. Пинкод должен состоять из 4 цифр.");
			string hashedPin = BCrypt.Net.BCrypt.HashPassword(pincode);
			Account newAcc = new Account {Mail = mail, Pincode = hashedPin };
			await _repository.Add(newAcc);
			return new AccountDto
			{
				Mail = newAcc.Mail,
				Balance = newAcc.Balance
			};
		}
		public async Task<bool> CheckPinAsync(Account account, string pincode, bool isUnbanning)
		{
			if (account.IsBanned && !isUnbanning)
			{
				throw new Exception("Аккаунт заблокирован.");
			}
			bool isCorrect = BCrypt.Net.BCrypt.Verify(pincode, account.Pincode);
			if (isCorrect)
			{
				account.FailedAttempts = 0;
				await _repository.Update(account);
				return true;
			}
			else
			{
				if (!account.IsBanned)
				{
					account.FailedAttempts++;
					if (account.FailedAttempts >= 3)
						account.IsBanned = true;
					await _repository.Update(account);
				}
				string message = account.IsBanned 
				? "Неверный пинкод. Аккаунт заблокирован."
				: $"Неверный пинкод. Осталось попыток: {3 - account.FailedAttempts}";
				throw new Exception(message);
			}
		}
		public async Task UnbanAsync(string mail,string pin)
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден");
			await CheckPinAsync(acc, pin, true);
			acc.IsBanned = false;
			acc.FailedAttempts = 0;
			await _repository.Update(acc);
		}

		public async Task TransferAsync(string targetMail, string mail, int amount)
		{
			var sender = await _repository.GetByMail(mail);
			if (sender == null)
				throw new Exception("Аккаунт не найден.");
			var receiver = await _repository.GetByMail(targetMail);
			if (receiver == null)
				throw new Exception("Аккаунт не найден.");
			if (sender.Id == receiver.Id)
				throw new Exception("Вы не можете совершить перевод самому себе.");
			if (amount < 1)
				throw new Exception("Сумма перевода должна быть больше 0.");
			if (amount > sender.Balance)
				throw new Exception("У вас недостаточно денежных средств для перевода.");
			receiver.Balance += amount;
			sender.Balance -= amount;
			await _repository.AddTransaction(new Transaction { AccountId = sender.Id, Type = "Перевод (исх)", Amount = -amount, TargetMail = receiver.Mail });
			await _repository.AddTransaction(new Transaction { AccountId = receiver.Id, Type = "Первеод (вх)", Amount = amount, TargetMail = sender.Mail });
			await _repository.Update(receiver);
			await _repository.Update(sender);
		}

		public async Task<List<TransactionDto>> GetHistoryAsync(string mail)
		{
			var acc = await _repository.GetByMail(mail);
			if (acc == null)
				throw new Exception("Аккаунт не найден.");
			var transactions =  await _repository.GetHistory(acc.Id);

			var historyDto = transactions.Select(t => new TransactionDto
			{
				Type = t.Type,
				Amount = t.Amount,
				TargetMail = t.TargetMail,
				CreatedAt = t.CreatedAt
			}).ToList();
			return historyDto;
		}
	}
}