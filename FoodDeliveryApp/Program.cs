using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<FoodDeliveryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FoodDeliveryApp.Data")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<FoodDeliveryContext>()
    .AddDefaultTokenProviders();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FoodDeliveryContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Customer", "Manager", "Worker", "Driver" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    context.Database.EnsureCreated(); // Ensures DB exists
    if (!context.Categories.Any())
    {
        var json = File.ReadAllText(Path.Combine(builder.Environment.WebRootPath, "menu.json"));
        var data = JsonConvert.DeserializeObject<dynamic>(json);

        foreach (var cat in data.Category)
        {
            context.Categories.Add(new Category
            {
                ImagePath = cat.ImagePath?.ToString(),
                Name = cat.Name?.ToString() ?? string.Empty
            });
        }
        await context.SaveChangesAsync();

        var categories = context.Categories.ToDictionary(c => c.Name!, c => c.Id);
        foreach (var food in data.Foods)
        {
            var categoryName = data.Category[(int)food.CategoryId].Name.ToString() ?? string.Empty;
            context.MenuItems.Add(new MenuItem
            {
                Title = food.Title?.ToString(),
                Description = food.Description?.ToString(),
                ImagePath = food.ImagePath?.ToString(),
                CategoryId = categories[categoryName],
                Price = (decimal)food.Price
            });
        }
        await context.SaveChangesAsync();
    }
}

app.Run();