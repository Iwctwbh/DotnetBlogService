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

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblPostsHistory> TblPostsHistories { get; set; }

    public virtual DbSet<TblPostsTagsMapping> TblPostsTagsMappings { get; set; }

    public virtual DbSet<TblResultGeneral> TblResultGenerals { get; set; }

    public virtual DbSet<TblTag> TblTags { get; set; }

    public virtual DbSet<TblTagsHistory> TblTagsHistories { get; set; }

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
                .HasConstraintName("tblPostsTagsMapping_ibfk_1");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("tblPostsTagsMapping_ibfk_2");
        });

        modelBuilder.Entity<TblResultGeneral>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblResultGeneral");

            entity.Property(e => e.ErrorCode).HasColumnType("int(11)");
            entity.Property(e => e.ErrorDesc).HasMaxLength(50);
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
