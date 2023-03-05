using Microsoft.EntityFrameworkCore;
using Shared;
using UsDbDownloader.Helper;

namespace UsDbDownloader.Data;

public class Repository
{
    private readonly IDbContextFactory<UltraToolsContext> _dbFactory;

    private static SongModel GetOrCreateSong(UltraToolsContext context, CompleteSong song)
    {
        return context.Songs.AsEnumerable().FirstOrDefault(song.IsSameAs) ?? context.Songs.Add(song.AsSongModel()).Entity;
    }

    public Repository(IDbContextFactory<UltraToolsContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<PlaylistModel>> GetPlaylistsAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return context.Playlists.Include(p => p.Songs).AsNoTracking().ToList();
    }

    public async Task CreatePlaylistAsync(string name)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        context.Playlists.Add(new PlaylistModel() {Name = name});
        await context.SaveChangesAsync();
    }

    public async Task AddSongToPlaylistAsync(PlaylistModel playlist, CompleteSong song)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var dbPlaylist = context.Playlists.FirstOrDefault(x => x.Id == playlist.Id) ??
                         context.Playlists.Add(new PlaylistModel() {Name = playlist.Name}).Entity;
        dbPlaylist.Songs.Add(GetOrCreateSong(context, song));
        await context.SaveChangesAsync();
    }

    public async Task RemoveSongFromPlaylistAsync(PlaylistModel playlist, CompleteSong song)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var dbPlaylist = context.Playlists.FirstOrDefault(x => x.Id == playlist.Id) ??
                         context.Playlists.Add(new PlaylistModel() {Name = playlist.Name}).Entity;
        dbPlaylist.Songs.Remove(GetOrCreateSong(context, song));
        await context.SaveChangesAsync();
    }

    public async Task AddSongsToPlaylistAsync(PlaylistModel playlist, IEnumerable<CompleteSong> songs)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var dbPlaylist = context.Playlists.FirstOrDefault(x => x.Id == playlist.Id) ??
                         context.Playlists.Add(new PlaylistModel() {Name = playlist.Name}).Entity;
        dbPlaylist.Songs.AddRange(songs.Select(x => GetOrCreateSong(context, x)));
        await context.SaveChangesAsync();
    }

    public async Task RemoveSongsFromPlaylistAsync(PlaylistModel playlist, IEnumerable<CompleteSong> songs)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var dbPlaylist = context.Playlists.FirstOrDefault(x => x.Id == playlist.Id) ??
                         context.Playlists.Add(new PlaylistModel() {Name = playlist.Name}).Entity;
        dbPlaylist.Songs.RemoveAll(x => songs.Any(y => y.IsSameAs(x)));
        await context.SaveChangesAsync();
    }

    public async Task RemovePlaylist(PlaylistModel selectedPlaylist)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        context.Playlists.Remove(context.Playlists.Single(x => x.Id == selectedPlaylist.Id));
        await context.SaveChangesAsync();
    }
}