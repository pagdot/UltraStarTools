using Shared;

namespace PlaylistCreator.Models;

public class SongWithScoreModel
{
    public SongWithScoreModel(UsToolsSong song, UsScore? score = null)
    {
        Song = song;
        Score = score;
    }

    public UsToolsSong Song { get; }
    public UsScore? Score { get; }
    
}