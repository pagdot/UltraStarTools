using Microsoft.EntityFrameworkCore;
using Shared;

namespace ScanSongLibrary.DbModel
{
    public partial class UltrastarContext : DbContext
    {
        public UltrastarContext(DbContextOptions<UltrastarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UsScore> UsScores { get; set; } = null!;
        public virtual DbSet<UsSong> UsSongs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsSong>(entity =>
            {
                entity.ToTable("us_songs");
            });
            
            modelBuilder.Entity<UsScore>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("us_scores");
                entity.Property(x => x.Date).HasConversion(x => x.ToUnix(), x => DateTimeExtensions.FromUnix(x));
            });
        }
    }
}
