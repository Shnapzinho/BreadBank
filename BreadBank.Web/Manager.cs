using Microsoft.AspNetCore.Components.Forms;

namespace BreadBank.Web
{
	public class Manager
	{
		private Repository _repository;
		public Manager(Repository repository) 
		{
			_repository = repository;
		}
		public int CheckBalance(Account account,string pin) 
		{
			CheckPin(account, pin, false);
			return account.balance;
		}
		public void Deposit(Account account, int input, string pin)
		{
			CheckPin(account, pin, false);
			if (input < 0)
				throw new Exception("Введена некорректная сумма пополнения.");
			account.balance += input;
			_repository.Update(account);
		}
		public void Withdraw(Account account, int input,string pin)
		{
			CheckPin(account, pin, false);
			if (input <= 0)
				throw new Exception("Cумма снятия денежных средств должна быть больше 0.");
			if (account.balance < input)
				throw new Exception("Недостаточно денежных средств.");
			account.balance -= input;
			_repository.Update(account);
		}
		public Account Registration(string mail, string pincode) 
		{
			if (_repository.GetByMail(mail) != null)
				throw new Exception("Аккаунт с такой почтой уже существует.");
			Account newAcc = new Account();
			if (!int.TryParse(pincode, out int result))
				throw new Exception("Неверный формат. Пинкод должен состоять из цифр.");
			if (pincode.Length != 4)
				throw new Exception("Неверный формат. Пинкод должен состоять из 4 цифр.");
			newAcc.mail = mail;
			newAcc.pincode = result;
			_repository.Add(newAcc);
			return newAcc;
		}
		public bool CheckPin(Account account, string pincode, bool isUnbanning)
		{
			if (account.isBanned && !isUnbanning)
			{
				throw new Exception("Аккаунт заблокирован.");
			}

			if (!int.TryParse(pincode, out int pin))
				throw new Exception("Неверный формат. Пинкод должен состоять из цифр.");

			if (pin == account.pincode)
			{
				account.failedAttempts = 0;
				_repository.Update(account);
				return true;
			}
			else
			{
				if (!account.isBanned)
				{
					account.failedAttempts++;
					if (account.failedAttempts >= 3)
						account.isBanned = true;
					_repository.Update(account);
				}
				string message = account.isBanned 
				? "Неверный пинкод. Аккаунт заблокирован."
				: $"Неверный пинкод. Осталось попыток: {3 - account.failedAttempts}";
				throw new Exception(message);
			}
		}
		public void Unban(Account account, string pincode)
		{
			CheckPin(account, pincode, true);
			account.isBanned = false;
			account.failedAttempts = 0;
			_repository.Update(account);
		}

		public void Transfer(Account receiver, Account sender, string pin, int amount)
		{
			CheckPin(sender,pin,false);
			if (sender.id == receiver.id)
				throw new Exception("Вы не можете совершить перевод самому себе.");
			if (amount < 1)
				throw new Exception("Сумма перевода должна быть больше 0.");
			if (amount > sender.balance)
				throw new Exception("У вас недостаточно денежных средств для перевода.");
			receiver.balance += amount;
			sender.balance -= amount;
			_repository.Update(receiver);
			_repository.Update(sender);
		}
	}
}