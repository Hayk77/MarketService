using MyProject.Interfaces;
using MyProject.Models;
using MyProject.Repositories;
using MyProject.Services;
using MyProject.Enums;


namespace MyProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User user = new User();
            Product product = new Product();
            IRepository repository = new Repository();
            IMarketService marketService = new MarketService(repository);
            IUserService userService = new UserService(repository);

            Console.WriteLine($"Welcome {marketService.GetMarketName()}!");

            try
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new Exception("Name is required!");
                }

                if (name == "admin" || name == "Admin")
                {
                    Console.Write("Password: ");
                    string password = Console.ReadLine();

                    while (true)
                    {
                        if (marketService.IsAdmin(password))
                        {
                            Console.WriteLine("1.Buy product | 2.Check balance | 3.Get all products | 4.Update product price | 5.Delete product" +
                            "                        6.Get all products quantity | 7.Get products sell income | 8.Get all orders details |0.Exit");
                            Console.Write("Select: ");
                            int input = int.Parse(Console.ReadLine());

                            AdminMenu option = (AdminMenu)input;

                            if (option == 0)
                            {
                                break;
                            }

                            switch (option)
                            {
                                case AdminMenu.BuyProduct:
                                    Console.Write("Product name: ");
                                    product.Name = Console.ReadLine();
                                    Console.Write("Product quantity: ");
                                    product.Quantity = int.Parse(Console.ReadLine());
                                    Console.Write("Price: ");
                                    product.Price = decimal.Parse(Console.ReadLine());

                                    if (marketService.BuyProductForMarket(product))
                                    {
                                        Console.WriteLine("Product purchased succsesfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Product purchased failed.");
                                    }
                                    break;

                                case AdminMenu.CheckBalance:
                                    decimal balance = marketService.GetMarketBalance();
                                    Console.WriteLine($"Balance: {balance}");
                                    break;

                                case AdminMenu.GetAllProducts:
                                    var products = marketService.GetAllProducts();

                                    foreach (var currentProduct in products)
                                    {
                                        Console.WriteLine($"{currentProduct.Name} | {currentProduct.Price}$ | {currentProduct.Quantity}");
                                    }
                                    break;

                                case AdminMenu.UpdateProductPrice:
                                    Console.Write("New price: ");
                                    decimal newPrice = decimal.Parse(Console.ReadLine());
                                    Console.Write("Product name: ");
                                    string productName = Console.ReadLine();

                                    if (marketService.UpdateProductPrice(newPrice, productName))
                                    {
                                        Console.WriteLine("Product price updated!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Product price updating failed.");
                                    }
                                    break;

                                case AdminMenu.DeleteProduct:
                                    Console.Write("Product name: ");
                                    string productNameForDelete = Console.ReadLine();

                                    if (marketService.DeleteProduct(productNameForDelete))
                                    {
                                        Console.WriteLine($"Product {productNameForDelete} deleted sucsessfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Product deleting failed.");
                                    }
                                    break;

                                case AdminMenu.GetAllProductsQuantity:
                                    int productsQuantity = marketService.GetAllProductsQuantity();
                                    Console.WriteLine($"All products quantity: {productsQuantity}");
                                    break;

                                case AdminMenu.GetProductsSellIncome:
                                    decimal income = marketService.GetProductsSellIncome();
                                    Console.WriteLine($"All products sell income: {income}");
                                    break;

                                case AdminMenu.GetAllOrderDetails:
                                    var allOrderDetails = marketService.GetAllOrderDetails();

                                    foreach (var currentOrder in allOrderDetails)
                                    {
                                        Console.WriteLine($"{currentOrder.ProductName} | {currentOrder.Quantity} | {currentOrder.Price} |" +
                                        $"{currentOrder.TotalPrice} | {currentOrder.UserName} | {currentOrder.Email} | {currentOrder.CreatedAt}");
                                    }
                                    break;

                            }
                        }
                        else
                        {
                            throw new Exception("Wrong password for admin.");
                        }
                    }
                }
                else
                {
                    Console.Write("Email: ");
                    string email = Console.ReadLine();

                    while (true)
                    {
                        if (!userService.IsCorrectEmailFormat(email))
                        {
                            throw new Exception("Incorrect email format!");
                        }

                        Console.WriteLine("1.Add user | 2.Get all products | 3.Buy product | 4.Add balance | 5.Get balance | 0.Exit");
                        Console.Write("Select: ");
                        int input = int.Parse(Console.ReadLine());

                        UserMenu option = (UserMenu)input;

                        if (option == 0)
                        {
                            break;
                        }

                        switch (option)
                        {
                            case UserMenu.AddUser:
                                user.Name = name;
                                user.Email = email;
                                Console.Write("Your balance: ");
                                user.Balance = decimal.Parse(Console.ReadLine());

                                if (userService.AddUser(user))
                                {
                                    Console.WriteLine($"Welcome {user.Name}!");
                                }
                                else
                                {
                                    Console.WriteLine("User adding failed.");
                                }
                                break;

                            case UserMenu.GetAllProducts:
                                var products = marketService.GetAllProducts();

                                foreach (var currentProduct in products)
                                {
                                    Console.WriteLine($"{currentProduct.Name} | {currentProduct.Price}$ | {currentProduct.Quantity}");
                                }
                                break;

                            case UserMenu.BuyProduct:
                                user.Email = email;

                                Console.Write("Product name: ");
                                string productNameForBuying = Console.ReadLine();
                                product.Name = productNameForBuying;

                                Console.Write("Quantity: ");
                                int quantity = int.Parse(Console.ReadLine());

                                if (userService.BuyProductForUser(user, product, quantity))
                                {
                                    Console.WriteLine("Product purchased succsesfully!");
                                }
                                else
                                {
                                    Console.WriteLine("Product purchased failed.");
                                }
                                break;

                            case UserMenu.AddBalance:
                                user.Email = email;
                                Console.Write("Adding price: ");
                                decimal price = decimal.Parse(Console.ReadLine());

                                if (userService.AddUserBalance(price, user))
                                {
                                    Console.WriteLine("Balance updated sucsessfully!");
                                }
                                else
                                {
                                    Console.WriteLine("Balance updating failed.");
                                }
                                break;

                            case UserMenu.GetBalance:
                                user.Email = email;

                                decimal userBalance = userService.GetUserBalance(user);
                                Console.WriteLine($"Balance: {userBalance}");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
