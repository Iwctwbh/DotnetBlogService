using DotnetBlogService.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlogService
{
    public class BlogScopedFactory : IDbContextFactory<BlogContext>
    {
        private const int DefaultTenantId = -1;

        private readonly IDbContextFactory<BlogContext> _pooledFactory;

        public BlogScopedFactory(
            IDbContextFactory<BlogContext> pooledFactory)
        {
            _pooledFactory = pooledFactory;
        }

        public BlogContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            return context;
        }
    }
}