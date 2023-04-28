using Microsoft.EntityFrameworkCore;

namespace DotnetBlogService.Models;

public partial class BlogContext : DbContext
{
    public BlogContext()
    {
    }

    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tblpost> Tblposts { get; set; }

    public virtual DbSet<TblpostsHistory> TblpostsHistories { get; set; }

    public virtual DbSet<Tbltag> Tbltags { get; set; }

    public virtual DbSet<TbltagsHistory> TbltagsHistories { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseMySql("server=;user id=;password=;database=", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.5-mariadb"));

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 根据环境变量合并配置文件
        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        var config = new ConfigurationModel(configurationRoot);

        optionsBuilder.UseMySql(config.GetConfig("ConnectionStrings:DefaultConnection"),
            ServerVersion.Parse("10.6.5-mariadb"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Tblpost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblposts");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedDate).HasMaxLength(6);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.ModifiedDate).HasMaxLength(6);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<TblpostsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblposts_history");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Action).HasMaxLength(10);
            entity.Property(e => e.DateCreated).HasMaxLength(6);
            entity.Property(e => e.DateModified).HasMaxLength(6);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.PostId).HasColumnType("int(11)");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Tbltag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbltags");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateModified).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TbltagsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tbltags_history");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Action).HasMaxLength(10);
            entity.Property(e => e.DateCreated).HasMaxLength(6);
            entity.Property(e => e.DateModified).HasMaxLength(6);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TagId).HasColumnType("int(11)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}