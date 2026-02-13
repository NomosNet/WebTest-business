using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebTest.BusinessLogic.Services;
using WebTest.DataAccess.Repositories;
using WebTest.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Business Logic Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, UserRole.Manager.ToString()));
    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, UserRole.Customer.ToString()));
});

var app = builder.Build();

// Apply database migrations / create DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new AppUser { UserName = "manager", Password = "manager", Role = UserRole.Manager },
            new AppUser { UserName = "customer", Password = "customer", Role = UserRole.Customer }
        );
        db.SaveChanges();
    }

    // Создаём заказчика для тестового пользователя customer
    var customerUser = db.Users.FirstOrDefault(u => u.UserName == "customer");
    if (customerUser != null && !db.Customers.Any(c => c.AppUserId == customerUser.Id))
    {
        var year = DateTime.UtcNow.Year;
        var codeNum = db.Customers.Count() + 1;
        db.Customers.Add(new Customer
        {
            Id = Guid.NewGuid(),
            Name = customerUser.UserName,
            Code = $"{codeNum:D4}-{year}",
            AppUserId = customerUser.Id,
            Discount = null
        });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
