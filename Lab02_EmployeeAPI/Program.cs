using Microsoft.EntityFrameworkCore;
using Lab02_EmployeeAPI.Data;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 註冊 Entity Framework Core 服務
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊 CORS 服務（讓前端可以呼叫 API）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 啟用 CORS
app.UseCors("AllowAll");

// 啟用靜態檔案服務（讓我們可以放 HTML, CSS, JS）
app.UseStaticFiles();

// 🎯 設定預設頁面為 index.html
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});


app.UseAuthorization();

app.MapControllers();

// 🎯 設定根路徑重導向到 index.html
app.MapFallbackToFile("index.html");


app.Run();