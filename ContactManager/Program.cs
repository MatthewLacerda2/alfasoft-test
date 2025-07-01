using Microsoft.EntityFrameworkCore;
using System.Data;
using Alfasoft.Context;

var builder = WebApplication.CreateBuilder(args);

// Configure MariaDB connection
var connectionString = builder.Configuration.GetConnectionString("MariaDB")
    ?? throw new InvalidOperationException("Connection string 'MariaDB' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// Add Razor Pages support
builder.Services.AddRazorPages();

var app = builder.Build();

// ✅ Ensure database is created and migrations applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<ApplicationDbContext>();

    if (dbContext != null && dbContext.Database.GetDbConnection().State != ConnectionState.Open)
    {
        dbContext.Database.OpenConnection();
        dbContext.Database.Migrate();
    }
}

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
