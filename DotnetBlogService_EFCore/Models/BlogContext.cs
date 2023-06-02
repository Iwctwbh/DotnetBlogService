using DotnetBlogService.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlogService_EFCore.Models;

public partial class BlogContext : DbContext
{
    public BlogContext()
    {
    }

    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblPostsHistory> TblPostsHistories { get; set; }

    public virtual DbSet<TblPostsTagsMapping> TblPostsTagsMappings { get; set; }

    public virtual DbSet<TblTag> TblTags { get; set; }

    public virtual DbSet<TblTagsHistory> TblTagsHistories { get; set; }

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

        modelBuilder.Entity<TblPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblPosts");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DateCreated).HasMaxLength(6);
            entity.Property(e => e.DateModified).HasMaxLength(6);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<TblPostsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblPosts_History");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Action).HasMaxLength(10);
            entity.Property(e => e.DateCreated).HasMaxLength(6);
            entity.Property(e => e.DateModified).HasMaxLength(6);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.PostId).HasColumnType("int(11)");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<TblPostsTagsMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblPostsTagsMapping");

            entity.HasIndex(e => e.PostId, "PostId");

            entity.HasIndex(e => e.TagId, "TagId");

            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.PostId).HasColumnType("int(11)");
            entity.Property(e => e.TagId).HasColumnType("int(11)");

            entity.HasOne(d => d.Post).WithMany()
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("tblpoststagsmapping_ibfk_1");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("tblpoststagsmapping_ibfk_2");
        });

        modelBuilder.Entity<TblTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblTags");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateModified).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TblTagsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tblTags_History");

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