using DotnetBlogService_EFCore.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/AddPost", (string argTitle, string argContent) =>
    {
        using var db = new BlogContext();
        var post = new TblPost
        {
            Title = argTitle,
            Content = argContent,
            DateCreated = DateTime.Now,
            DateModified = DateTime.Now,
            IsActive = 1
        };
        db.TblPosts.Add(post);
        db.SaveChanges(); // 提交更改后才能保存到数据库。
    })
    .WithName("AddPost")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}