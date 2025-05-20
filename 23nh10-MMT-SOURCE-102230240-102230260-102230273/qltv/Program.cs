using Microsoft.Extensions.FileProviders;
using qltv.Data; // DbContext
using Microsoft.EntityFrameworkCore;
using System.IO;
using qltv.Models; // Models
using qltv.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Đăng ký DbContext với InMemory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Register services
builder.Services.AddScoped<IMockDataService, MockDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// HTTPS redirect
app.UseHttpsRedirection();

// 4. Static files cho wwwroot
app.UseStaticFiles(); // phục vụ wwwroot (mặc định)

// 5. Static files cho thư mục wwwroot/react tại URL /react
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "react")),
    RequestPath = "/react"
});

// 6. Fallback cho React Router (Single Page App)
app.MapFallbackToFile("/react/{*path:nonfile}", "react/index.html");

// 7. Routing
app.UseRouting();

// 8. Authorization (nếu dùng)
app.UseAuthorization();

// 9. Map route cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseCors("AllowAll");

app.Run();
