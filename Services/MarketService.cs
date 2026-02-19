using MyProject.Interfaces;
using MyProject.Models;
using MyProject.PasswordHashers;
using System.Data;

namespace MyProject.Services
{
    public class MarketService : IMarketService
    {
        private readonly IRepository _repository;
        private readonly RedisCacheService _cache;

        public MarketService(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = new RedisCacheService();
        }

        public bool IsAdmin(string password)
        {
            var (hash, salt) = _repository.GetAdminPassword();
            return PasswordHasher.Verify(password, hash, salt);
        }

        public string GetMarketName()
        {
            var cacheKey = "market:name";
            var cached = _cache.Get(cacheKey);

            if (cached != null)
                return cached;

            var market = _repository.GetAll<Market>().FirstOrDefault(m => m.Id == 1);
            if (market == null)
            {
                throw new Exception("Market not found!");
            }

            _cache.Set(cacheKey, market.Name, TimeSpan.FromMinutes(5));
            return market.Name;
        }

        public decimal GetMarketBalance()
        {
            var cacheKey = "market:balance";
            var cached = _cache.Get(cacheKey);

            if (cached != null)
                return decimal.Parse(cached);

            var market = _repository.GetAll<Market>().FirstOrDefault(m => m.Id == 1);
            if (market == null)
            {
                throw new Exception("Market not found!");
            }

            _cache.Set(cacheKey, market.Balance.ToString(), TimeSpan.FromMinutes(5));
            return market.Balance;
        }

        public int GetAllProductsQuantity()
        {
            return _repository.GetAll<Product>().Sum(p => p.Quantity);
        }

        public decimal GetProductsSellIncome()
        {
            var orders = _repository.GetAll<Order>();
            return orders.Sum(o => o.TotalPrice);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _repository.GetAll<Product>().Where(p => p.IsActive);
        }

        public bool UpdateProductPrice(decimal newPrice, string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new Exception("Product name is required!");

            if (newPrice < 0)
                throw new Exception("Price cannot be negative!");

            var product = _repository.GetAll<Product>().FirstOrDefault(p => p.Name == productName);
            if (product == null)
                throw new Exception("Product not found!");

            product.Price = newPrice;
            _repository.Update(product);
            return true;
        }

        public bool DeleteProduct(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new Exception("Product name is required!");

            var product = _repository.GetAll<Product>().FirstOrDefault(p => p.Name == productName);
            if (product == null)
                throw new Exception("Product not found!");

            product.IsActive = false;
            _repository.Update(product);
            return true;
        }

        public bool BuyProductForMarket(Product product)
        {
            if (product.Price <= 0)
                throw new Exception("Price must be greater than zero");

            if (product.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            if (_repository.BuyProductForMarket(product))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<OrderDetailsDto> GetAllOrderDetails()
        {
            return _repository.GetOrderDetails();
        }

        public TopSoldProductDto GetMostSoldProduct()
        {
            var result = _repository.GetTopSoldProduct();

            if (result == null)
                throw new Exception("No sales found");

            return result;
        }
    }

}
