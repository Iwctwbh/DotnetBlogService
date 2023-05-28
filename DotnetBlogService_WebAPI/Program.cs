using DotnetBlogService_EFCore.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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

        //var param1 = new SqlParameter("@param1", "value1");
        //var param2 = new SqlParameter("@param2", "value2");

        //var result = BlogContext.FromSqlRaw("EXECUTE MyStoredProc @param1, @param2", param1, param2).ToList();

        using var context = new BlogContext();
        var parameter0 = new MySqlParameter("@inputParam", "someValue");
        var parameter1 = new MySqlParameter("@inputParam", "someValue");
        var parameter2 = new MySqlParameter("@inputParam", "someValue");

        var result = context.TblPosts.FromSqlRaw("CALL sp_AddPost({0}, {1}, {2})", parameter0, parameter1, parameter2)
            .ToList();
    })
    .WithName("AddPost")
    .WithOpenApi();

app.MapGet("/GetPosts", (int argTake, int argSkip) =>
    {
        var sqlResult = new BlogContext().TblPosts.AsNoTracking().Where(w => w.IsActive == 1).Skip(argSkip)
            .Take(argTake).ToList();
        return sqlResult;
    })
    .WithName("GetPosts")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}