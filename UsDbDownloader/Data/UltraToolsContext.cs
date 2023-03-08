using Microsoft.EntityFrameworkCore;

namespace UsDbDownloader.Data;

public class UltraToolsContext : DbContext
{
    public UltraToolsContext()
    {}
    
    public UltraToolsContext(DbContextOptions<UltraToolsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SongModel> Songs { get; set; } = null!;
    public virtual DbSet<PlaylistModel> Playlists { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }
    
}