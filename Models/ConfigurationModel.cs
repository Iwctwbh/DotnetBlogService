namespace DotnetBlogService.Models;

public class ConfigurationModel
{
    // requires using Microsoft.Extensions.Configuration;
    private readonly IConfiguration _configuration;

    public ConfigurationModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConfig(string argKey)
    {
        var value = _configuration[argKey];

        return value ?? "";
    }
}