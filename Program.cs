using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Data;
using SimpleApiProject.Repositories;
using SimpleApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register AppDbContext with PostgreSQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Dependency injection for repository and service layers
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Add controllers to the service container
builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Use authorization middleware
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Test database connection on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.CanConnect();
        Console.WriteLine(" PostgreSQL baðlantýsý baþarýlý!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Veritabaný baðlantý hatasý: {ex.Message}");
    }
}

app.Run();
