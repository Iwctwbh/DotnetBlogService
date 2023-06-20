using DotnetBlogService.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

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

//builder.Services.AddDbContext<BlogContext>(options =>
//{
//    options.UseMySql(builder.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection"),
//        ServerVersion.Parse("10.6.5-mariadb"));
//});

// 数据库连接池
builder.Services.AddPooledDbContextFactory<BlogContext>(optionsBuilder =>
    optionsBuilder.UseMySql(builder.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection"),
        ServerVersion.Parse("10.6.5-mariadb")));

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

    List<TblPost> result = context.TblPosts.FromSqlRaw("CALL sp_AddPost({0}, {1}, {2})", parameter0, parameter1, parameter2)
        .ToList();
})
    .WithName("AddPost")
    .WithOpenApi();

var maxMessageSize = 80 * 1024;


app.MapPost("/GetPosts", async (HttpRequest req, Stream argBody, BlogContext db) =>
{
    var skip = 0;

    if (req.ContentLength is not null && req.ContentLength > maxMessageSize) return await db.TblPosts.AsNoTracking().Where(w => w.IsActive == 1).Skip(0).Take(10).ToListAsync();

    var readSize = (int?)req.ContentLength ?? maxMessageSize + 1;

    var buffer = new byte[readSize];

    var read = await argBody.ReadAtLeastAsync(buffer, readSize, false);
    if (read > maxMessageSize) return await db.TblPosts.AsNoTracking().Where(w => w.IsActive == 1).Skip(0).Take(10).ToListAsync();
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
    int.TryParse(jsonBody.Property("skip")?.Value.ToString(), out skip);
    skip = Math.Max(0, skip);

    var a = await db.TblPosts.AsNoTracking().Where(w => w.IsActive == 1).Skip(skip).Take(10).ToListAsync();
    var b = JsonSerializer.Serialize(a);
    return a;
});

//app.MapPost("/register", async (HttpRequest req, Stream body,
//    Channel<ReadOnlyMemory<byte>> queue) =>
//{
//    if (req.ContentLength is not null && req.ContentLength > maxMessageSize) return Results.BadRequest();

//    // We're not above the message size and we have a content length, or
//    // we're a chunked request and we're going to read up to the maxMessageSize + 1. 
//    // We add one to the message size so that we can detect when a chunked request body
//    // is bigger than our configured max.
//    var readSize = (int?)req.ContentLength ?? maxMessageSize + 1;

//    var buffer = new byte[readSize];

//    // Read at least that many bytes from the body.
//    var read = await body.ReadAtLeastAsync(buffer, readSize, false);
//    var ssss = Encoding.Default.GetString(buffer);
//    // We read more than the max, so this is a bad request.
//    if (read > maxMessageSize) return Results.BadRequest();

//    // Attempt to send the buffer to the background queue.
//    if (queue.Writer.TryWrite(buffer.AsMemory(..read))) return Results.Accepted();

//    // We couldn't accept the message since we're overloaded.
//    return Results.StatusCode(StatusCodes.Status429TooManyRequests);
//});

app.Run();