using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Kursova_VideoStore.Models;
using Kursova_VideoStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQL Server
builder.Services.AddDbContext<VideotekaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VideotekaContext")));

// Register Identity with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<VideotekaContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VideotekaContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync(); 

    await DbInitializer.InitializeAsync(context, userManager, roleManager);
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // required for Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();       // required for Identity UI

app.Run();
