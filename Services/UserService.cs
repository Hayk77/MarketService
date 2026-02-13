using MyProject.Interfaces;
using MyProject.Models;
using System.Text.RegularExpressions;


namespace MyProject.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository)
        {
            _repository = repository;
        }

        public bool AddUser(User user)
        {
            if (!IsNameStartedWithUpperCase(user.Name))
                throw new Exception("Name need to start with upper case!");

            if (!IsCorrectEmailFormat(user.Email))
                throw new Exception("Incorrect email format!");

            if (!IsUnrepetableEmail(user.Email))
                throw new Exception("Email already exists!");

            if (user.Balance < 0)
                throw new Exception("Balance couldn't be negative!");

            user.IsActive = true;

            _repository.Add(user);
            return true;
        }

        public bool BuyProductForUser(User user, Product product, int quantity)
        {
            if (_repository.BuyProductForUser(user, product, quantity))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddUserBalance(decimal amount, User user)
        {
            var dbUser = _repository.Get<User>(u => u.Email == user.Email);

            if (dbUser == null)
                throw new Exception("User doesn't exist!");

            if (amount <= 0)
                throw new Exception("Amount must be positive!");

            dbUser.Balance += amount;
            _repository.Update(dbUser);

            return true;
        }

        public decimal GetUserBalance(User user)
        {
            var dbUser = _repository.Get<User>(u => u.Email == user.Email);

            if (dbUser == null)
                throw new Exception("User doesn't exist!");

            return dbUser.Balance;
        }

        public bool IsNameStartedWithUpperCase(string name)
        {
            return Regex.IsMatch(name, @"^[A-Z]");
        }

        public bool IsCorrectEmailFormat(string email)
        {
            return !string.IsNullOrWhiteSpace(email)
                && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool IsUnrepetableEmail(string email)
        {
            return !_repository
                .GetAll<User>()
                .Any(u => u.Email == email);
        }
    }
}
