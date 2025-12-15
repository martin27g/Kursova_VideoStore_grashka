using Videoteka.Models;
using Microsoft.AspNetCore.Identity;

namespace Kursova_VideoStore.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(VideotekaContext context,
                                                 UserManager<IdentityUser> userManager,
                                                 RoleManager<IdentityRole> roleManager)
        {
            // --- Seed domain data (Films, Customers, Employees, Orders, OrderDetails) ---
            if (!context.Films.Any())
            {
                var films = new List<Film>
                {
                    new Film { Title="The Matrix", Genre="Sci-Fi", ReleaseYear=1999, Price=9.99m, Stock=10 },
                    new Film { Title="Inception", Genre="Sci-Fi", ReleaseYear=2010, Price=12.50m, Stock=5 },
                    new Film { Title="The Godfather", Genre="Crime", ReleaseYear=1972, Price=15.00m, Stock=3 }
                };
                context.Films.AddRange(films);
                context.SaveChanges();

                var customers = new List<Customer>
                {
                    new Customer { FirstName="Ivan", LastName="Petrov", Email="ivan@example.com", Phone="0888123456", Address="Sofia" },
                    new Customer { FirstName="Maria", LastName="Georgieva", Email="maria@example.com", Phone="0888765432", Address="Plovdiv" }
                };
                context.Customers.AddRange(customers);
                context.SaveChanges();

                var employees = new List<Employee>
                {
                    new Employee { FirstName="Georgi", LastName="Dimitrov", Position="Sales", HireDate=DateTime.Now.AddYears(-2) },
                    new Employee { FirstName="Elena", LastName="Koleva", Position="Manager", HireDate=DateTime.Now.AddYears(-5) }
                };
                context.Employees.AddRange(employees);
                context.SaveChanges();

                var orders = new List<Order>
                {
                    new Order { CustomerID = customers[0].CustomerID, EmployeeID = employees[0].EmployeeID, OrderDate = DateTime.Now },
                    new Order { CustomerID = customers[1].CustomerID, EmployeeID = employees[1].EmployeeID, OrderDate = DateTime.Now }
                };
                context.Orders.AddRange(orders);
                context.SaveChanges();

                var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail { OrderID = orders[0].OrderID, FilmID = films[0].FilmID, Quantity = 1 },
                    new OrderDetail { OrderID = orders[1].OrderID, FilmID = films[1].FilmID, Quantity = 2 }
                };
                context.OrderDetails.AddRange(orderDetails);
                context.SaveChanges();
            }

            // --- Seed Identity roles and Admin user ---
            if (!await roleManager.RoleExistsAsync(Roles.AdminEndUser))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.AdminEndUser));
            }

            var adminEmail = "admin@videoteka.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.AdminEndUser);
                }
            }
        }
    }
}
