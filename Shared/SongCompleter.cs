using Microsoft.EntityFrameworkCore;
using Shared.DbModel;

namespace Shared;

public class SongCompleter
{
    private readonly List<UsScore> _scores;
    private readonly List<UsSong> _songs;

    public SongCompleter(UltrastarContext context)
    {
        _scores = context.UsScores.Include(x => x.Song).ToList();
        _songs = context.UsSongs.ToList();
    }

    public IEnumerable<CompleteSong> CompleteCrawledSongs(IEnumerable<CrawledSong> songs) =>
        songs.Select(crawledSong => {
            var song = _songs.FirstOrDefault(song =>
                song.Artist.Equals(crawledSong.Artist, StringComparison.InvariantCultureIgnoreCase) &&
                song.Title.Equals(crawledSong.Title, StringComparison.InvariantCultureIgnoreCase));
            
            var highscore = _scores.OrderByDescending(x => x.Score).FirstOrDefault(x => 
                x.Song.Artist.Equals(crawledSong.Artist, StringComparison.InvariantCultureIgnoreCase) &&
                x.Song.Title.Equals(crawledSong.Title, StringComparison.InvariantCultureIgnoreCase));

            return new CompleteSong(crawledSong.Artist, crawledSong.Title, song?.TimesPlayed, song?.Rating, highscore?.Score, highscore?.Player, highscore?.Date);
        });
}
