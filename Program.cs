using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DMAWS_T2305M_PhamDangTung.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ cho ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ điều khiển
builder.Services.AddControllers();

// Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Management API", Version = "v1" });
});

var app = builder.Build();

// Cấu hình Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Sử dụng Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
    c.RoutePrefix = string.Empty; // Đặt Swagger UI tại trang gốc
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
