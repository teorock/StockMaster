using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMaster.Data;
using StockMaster.Models.Identity;
using Microsoft.AspNetCore.Identity;

using StockMaster.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") 
    ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(identityConnectionString));

// Registrazione StockDbContext per il database magazzino
var stockConnectionString = builder.Configuration.GetConnectionString("StockConnection") 
    ?? throw new InvalidOperationException("Connection string 'StockConnection' not found.");

builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseSqlite(stockConnectionString));

builder.Services.AddScoped<StockMaster.Services.Stock.MagazzinoService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity Service con ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// HttpContextAccessor per audit trail
builder.Services.AddHttpContextAccessor();

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CanManageStock", policy => policy.RequireRole("Admin", "Manager", "Operatore"));
    options.AddPolicy("CanEditAnagrafiche", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("CanManageUsers", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Magazzino}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}")
    pattern: "{controller=Magazzino}/{action=Index}/{id?}")
    .WithStaticAssets();


// Seed Identity Data (Ruoli e Admin iniziale)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await IdentitySeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante seed Identity");
    }
}

app.Run();