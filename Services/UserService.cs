namespace ShoppingList.Services
{
	public class UserService
	{
		public bool IsPasswordValid(string password)
		{
			if (password.Length < 8)
				return false;
			bool hasUppercase = false;
			bool hasLowercase = false;
			bool hasDigit = false;
			foreach (char c in password)
			{
				if (char.IsUpper(c))
					hasUppercase = true;
				else if (char.IsLower(c))
					hasLowercase = true;
				else if (char.IsDigit(c))
					hasDigit = true;
			}
			return hasUppercase && hasLowercase && hasDigit;
		}
	}
}
