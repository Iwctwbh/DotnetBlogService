using DotnetBlogService.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using DotnetBlogService;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddControllers();

// Configure JSON logging to the console.
//builder.Logging.AddJsonConsole();

// Add the memory cache services.
//builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy1",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                //.WithOrigins("http://localhost:3000")
                .AllowAnyOrigin()
                .AllowAnyHeader();
        });
});

//builder.Services.AddDbContextPool<BlogContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("10.3.28-mariadb"), mySqlDbContextOptionsBuilder => mySqlDbContextOptionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// 数据库连接池工厂模式
builder.Services.AddPooledDbContextFactory<BlogContext>(optionsBuilder => optionsBuilder.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("10.3.28-mariadb"), mySqlDbContextOptionsBuilder => mySqlDbContextOptionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

builder.Services.AddScoped<BlogScopedFactory>();

builder.Services.AddScoped(sp => sp.GetRequiredService<BlogScopedFactory>().CreateDbContext());

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();
app.UseCors("Policy1");
//app.UseEndpoints(e => { });
//app.Use((context, next) => next(context));
//app.UseRouting();
//app.UseDefaultFiles();
//app.UseStaticFiles();
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

const int maxMessageSize = 80 * 1024;

app.MapPost("/AddPost", async (HttpRequest req, Stream argBody, BlogContext db) =>
{
    //var post = new TblPost
    //{
    //    Title = argTitle,
    //    Content = argContent,
    //    DateCreated = DateTime.Now,
    //    DateModified = DateTime.Now,
    //    IsActive = 1
    //};
    //db.TblPosts.Add(post);
    //db.SaveChanges(); // 提交更改后才能保存到数据库。

    if (req.ContentLength is not null && req.ContentLength > maxMessageSize) return await db.TblResultGenerals.AsNoTracking().Take(10).ToListAsync();

    var readSize = (int?)req.ContentLength ?? maxMessageSize + 1;

    var buffer = new byte[readSize];

    var read = await argBody.ReadAtLeastAsync(buffer, readSize, false);
    if (read > maxMessageSize) return await db.TblResultGenerals.AsNoTracking().Take(10).ToListAsync();
    var bufferString = Encoding.Default.GetString(buffer);

    bufferString = System.Text.RegularExpressions.Regex.Unescape(bufferString);
    bufferString = bufferString.Trim('"');

    var jsonBody = new JObject();
    try
    {
        jsonBody = JObject.Parse(bufferString);
    }
    catch (Exception e)
    {
        // ignored
    }

    string? title = jsonBody.Property("title")?.Value.ToString();
    string? content = jsonBody.Property("content")?.Value.ToString();

    var result = await db.TblResultGenerals.FromSql($@"CALL sp_AddPost ({title}, {content}, 1)").ToListAsync();
    return result;
})
    .WithName("AddPost")
    .WithOpenApi();

app.MapPost("/GetPosts", async (HttpRequest req, Stream argBody, BlogContext db) =>
{
    if (req.ContentLength is not null && req.ContentLength > maxMessageSize) return JsonConvert.SerializeObject(new { Erroc = "ContentLength is too long." });

    var readSize = (int?)req.ContentLength ?? maxMessageSize + 1;

    var buffer = new byte[readSize];

    var read = await argBody.ReadAtLeastAsync(buffer, readSize, false);
    if (read > maxMessageSize) return JsonConvert.SerializeObject(new { Erroc = "ContentLength is too long." });
    var bufferString = Encoding.Default.GetString(buffer);

    bufferString = System.Text.RegularExpressions.Regex.Unescape(bufferString);
    bufferString = bufferString.Trim('"');

    var jsonBody = new JObject();
    try
    {
        jsonBody = JObject.Parse(bufferString);
    }
    catch (Exception e)
    {
        // ignored
    }

    // value为数字时，长度过长直接报错
    //var c = jsonBody.Value<string>("skip");
    int.TryParse(jsonBody.Property("skip")?.Value.ToString(), out int skip);
    int.TryParse(jsonBody.Property("take")?.Value.ToString(), out int take);
    string? order = jsonBody.Property("order")?.Value.ToString();
    string? search = jsonBody.Property("search")?.Value.ToString();
    skip = Math.Max(0, skip);
    take = jsonBody.ContainsKey("take") && take >= 0 ? take : 10;

    List<TblPost> result;

    if (order != null && order.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
    {
        result = await db.TblPosts.AsNoTracking()
            .Where(w => w.IsActive == 1 && (search == null || (EF.Functions.Like(w.Title, $"%{search}%") || EF.Functions.Like(w.Content, $"%{search}%"))))
            .OrderByDescending(o => o.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    result = await db.TblPosts.AsNoTracking()
        .Where(w => w.IsActive == 1 && (search == null || (EF.Functions.Like(w.Title, $"%{search}%") || EF.Functions.Like(w.Content, $"%{search}%"))))
        .Skip(skip)
        .Take(take)
        .ToListAsync();

    var count = await db.TblPosts.AsNoTracking().Where(w => w.IsActive == 1).CountAsync();

    return JsonConvert.SerializeObject(new { totalCount = count, data = result });
});

app.MapPost("/GetPostsTotalCount", async (BlogContext db) =>
{
    return await db.TblPosts.AsNoTracking().Where(w => w.IsActive == 1).CountAsync();
});

app.Run();