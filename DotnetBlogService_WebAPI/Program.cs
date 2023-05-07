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


// ���ݻ��������ϲ������ļ�
//var configurationRoot = new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json", true, true)
//    .AddEnvironmentVariables()
//    .Build();

//var config = new ConfigurationModel(configurationRoot);
//// ����MySQL���ݿ⼰���ӳ�
//builder.Services.AddDbContextPool<BlogContext>(option =>
//    option.UseMySql(config.GetConfig("ConnectionStrings:DefaultConnection"), ServerVersion.Parse("8.0.26-mysql")));

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

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
        db.SaveChanges(); // �ύ���ĺ���ܱ��浽���ݿ⡣
    })
    .WithName("AddPost")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}