using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultSQLiteConnection")));

builder.Services.AddControllers().AddControllersAsServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Medals API",
        Version = "v1",
        Description = "Olympic Medals API",
    });
    c.EnableAnnotations();
});

// Add CORS services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Open",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // Allow your local frontend
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// THIS IS THE KEY CHANGE -> UseCors must come before UseAuthorization
app.UseCors("Open");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();