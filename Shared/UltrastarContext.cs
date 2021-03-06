using Microsoft.EntityFrameworkCore;

namespace Shared
{
    public partial class UltrastarContext : DbContext
    {
        public UltrastarContext(DbContextOptions<UltrastarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UsScore> UsScores { get; set; } = null!;
        public virtual DbSet<UsSong> UsSongs { get; set; } = null!;
        public virtual DbSet<UsStatisticsInfo> UsStatisticsInfos { get; set; } = null!;
        public virtual DbSet<UsUsersInfo> UsUsersInfos { get; set; } = null!;
        public virtual DbSet<UsWeb> UsWebs { get; set; } = null!;
        public virtual DbSet<UsWebsStat> UsWebsStats { get; set; } = null!;
        public virtual DbSet<UsToolsSong> UsToolsSongs { get; set; } = null!;
        public virtual DbSet<UsToolsPlaylist> UsToolsPlaylists { get; set; } = null!;
        // public virtual DbSet<UsToolsPlaylistsSongs> UsToolsPlaylistsSongs { get; set; } = null!;

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

                entity.Property(e => e.Date).HasConversion(d => ToUnix(d), d => FromUnix(d));

                entity.ToTable("us_scores");
            });

            modelBuilder.Entity<UsStatisticsInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("us_statistics_info");
            });

            modelBuilder.Entity<UsUsersInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("us_users_info");

                entity.Property(e => e.WebId).HasColumnName("WebID");
            });

            modelBuilder.Entity<UsWeb>(entity =>
            {
                entity.ToTable("us_webs");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<UsWebsStat>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("us_webs_stats");

                entity.Property(e => e.MaxScore0).HasColumnName("Max_Score_0");

                entity.Property(e => e.MaxScore1).HasColumnName("Max_Score_1");

                entity.Property(e => e.MaxScore2).HasColumnName("Max_Score_2");

                entity.Property(e => e.MediaScore0).HasColumnName("Media_Score_0");

                entity.Property(e => e.MediaScore1).HasColumnName("Media_Score_1");

                entity.Property(e => e.MediaScore2).HasColumnName("Media_Score_2");

                entity.Property(e => e.SongId).HasColumnName("SongID");

                entity.Property(e => e.UserScore0).HasColumnName("User_Score_0");

                entity.Property(e => e.UserScore1).HasColumnName("User_Score_1");

                entity.Property(e => e.UserScore2).HasColumnName("User_Score_2");

                entity.Property(e => e.WebId).HasColumnName("WebID");
            });

            modelBuilder.Entity<UsToolsPlaylist>(entity =>
            {
                entity.HasMany(e => e.Songs).WithMany(e => e.Playlists);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
        private static long ToUnix(DateTime dateTime)
        {
            return dateTime.Subtract(DateTime.UnixEpoch.ToLocalTime()).Seconds;
        }
    
        private static DateTime FromUnix(long sec)
        {
            // Unix timestamp is seconds past epoch
            var dateTime = DateTime.UnixEpoch;
            dateTime = dateTime.AddSeconds(sec);
            return dateTime;
        }
    }
}
