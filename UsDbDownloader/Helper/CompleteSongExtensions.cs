using Shared;
using UsDbDownloader.Data;

namespace UsDbDownloader.Helper;

public static class CompleteSongExtensions
{
    public static bool IsSameAs(this CompleteSong me, CompleteSong song)
    {
        return me.Title == song.Title && me.Artist == song.Artist;
    }

    public static bool IsSameAs(this CompleteSong me, SongModel song)
    {
        return me.Title == song.Title && me.Artist == song.Artist;
    }

    public static SimpleScore? HighScore(this CompleteSong me) => me.Scores?.OrderByDescending(s => s.Score).FirstOrDefault();


    public static SongModel AsSongModel(this CompleteSong me)
    {
        return new SongModel
        {
            Artist = me.Artist,
            Title = me.Title
        };
    }
}
