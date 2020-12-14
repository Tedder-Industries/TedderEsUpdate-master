using Microsoft.EntityFrameworkCore;

namespace TedderEsUpdate.Models
{
    public partial class AghMageContext : DbContext
    {
        public AghMageContext()
        {
        }

        public AghMageContext(DbContextOptions<AghMageContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AWBlog> AwBlog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //staging
                optionsBuilder.UseMySql("server=aghmagestaging.ci5pmg00euyd.us-west-2.rds.amazonaws.com;port=4566;database=aghMageStaging;user=admin;password=e10BIs247sy9;treattinyasboolean=true", x => x.ServerVersion("5.6.34-mysql"));
                //production
                //optionsBuilder.UseMySql("server=cluster-agh.cluster-ci5pmg00euyd.us-west-2.rds.amazonaws.com;port=4566;database=aghMage;user=m2rPU7p2XSyEhkdu;password=9Wo7Xd4Tfhecoyp8EDQCdkEQG8kthA;treattinyasboolean=true", x => x.ServerVersion("5.6.10-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AWBlog>(entity =>
            {
                entity.HasKey(e => e.PostId)
                    .HasName("PRIMARY");

                entity.ToTable("aw_blog");

                entity.HasIndex(e => e.Identifier)
                    .HasName("identifier")
                    .IsUnique();

                entity.Property(e => e.PostId)
                    .HasColumnName("post_id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Author)
                    .HasColumnName("author")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("tinyint(11)");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("created_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Identifier)
                    .IsRequired()
                    .HasColumnName("identifier")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MetaDescription)
                    .IsRequired()
                    .HasColumnName("meta_description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MetaKeywords)
                    .IsRequired()
                    .HasColumnName("meta_keywords")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PostContent)
                    .IsRequired()
                    .HasColumnName("post_content")
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RelatedPosts)
                    .HasColumnName("related_posts")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ShortContent)
                    .IsRequired()
                    .HasColumnName("short_content")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ShorterContent)
                    .HasColumnName("shorter_content")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasColumnName("tags")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateUser)
                    .IsRequired()
                    .HasColumnName("update_user")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasColumnName("user")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}