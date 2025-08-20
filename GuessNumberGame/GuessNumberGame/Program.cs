var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

// 重要：設定靜態檔案服務
app.UseDefaultFiles();    // 設定預設檔案為 index.html
app.UseStaticFiles();     // 支援靜態檔案


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
