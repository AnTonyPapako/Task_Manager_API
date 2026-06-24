using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Connection string: παίρνουμε τα στοιχεία από env variables ---
// (ίδια λογική με το load_dotenv() + os.getenv() στο Python)
string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "taskuser";
string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "taskpassword";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "taskdb";

string connectionString =
    $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";

// --- Services ---
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "Ένα απλό REST API για διαχείριση εργασιών (To-Do Tasks)",
    });
});

var app = builder.Build();

// Δημιουργεί τον πίνακα tasks αν δεν υπάρχει ήδη
// (στα μεγαλύτερα production projects θα χρησιμοποιούσαμε EF Core Migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    c.RoutePrefix = "swagger"; // διαθέσιμο στο /swagger
});

app.MapControllers();

app.MapGet("/", () => new { message = "Task Management API is running" });

app.Run();
