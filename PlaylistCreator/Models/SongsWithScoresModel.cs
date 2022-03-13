using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlaylistCreator.Annotations;
using PlaylistCreator.Pages;
using Shared;

namespace PlaylistCreator.Models;

public class SongsWithScoresModel : INotifyPropertyChanged
{
  private bool _ascending = true;
  private SortingType _orderedBy = SortingType.Id;
  private string _filter = string.Empty;

  public enum SortingType
    {
        Id,
        Artist,
        Title,
        Score,
        Player,
        Date
    }
    
    public List<SongWithScoreModel> SongsWithScores { get; }
    public List<SongWithScoreModel> SongsWithScoresOrdered { private set; get; }
    public List<SongWithScoreModel> SongsWithScoresOrderedAndFiltered { private set; get; }

    public string Filter
    {
      get => _filter;
      set
      {
        if (value == _filter) return;
        _filter = value;
        OnPropertyChanged(nameof(Filter));
      }
    }

    public bool Ascending
    {
      set
      {
        if (value == _ascending) return;
        _ascending = value;
        OnPropertyChanged(nameof(Ascending));
      }
      get => _ascending;
    }

    public SortingType OrderedBy
    {
      set
      {
        if (value == _orderedBy) return;
        
        Ascending = true;
        _orderedBy = value;
        OnPropertyChanged(nameof(OrderedBy));
      }
      get => _orderedBy;
    }

    public SongsWithScoresModel(IEnumerable<UsToolsSong> songs, IEnumerable<UsScore> scores)
    {
        SongsWithScores = SongsWithScoresOrdered = SongsWithScoresOrderedAndFiltered = 
            songs.Select(song => new SongWithScoreModel(
                song,
                scores.FirstOrDefault(score => score.Song.Artist.Trim('\0') == song.Artist && score.Song.Title.Trim('\0') == song.Title))).ToList();

        PropertyChanged += async (sender, args) =>
        {
          if (args.PropertyName is nameof(Ascending) or nameof(OrderedBy)) SortItems();
        };
        
        PropertyChanged += async (sender, args) =>
        {
          if (args.PropertyName is nameof(Filter) or nameof(SongsWithScoresOrdered)) ApplyFilter();
        };
    }
    
    

  private void ApplyFilter()
  {
    SongsWithScoresOrderedAndFiltered = string.IsNullOrWhiteSpace(Filter) 
      ? SongsWithScoresOrdered 
      : SongsWithScoresOrdered.Where(s => Filter.Split(' ').All(x => 
        s.Song.Artist.ToLower().Contains(x.ToLower()) || s.Song.Title.ToLower().Contains(x.ToLower()) || (s.Score?.Player.ToLower().Contains(x.ToLower()) ?? false))).ToList();

    OnPropertyChanged(nameof(SongsWithScoresOrderedAndFiltered));
  }

  private void SortItems()
  {
    Func<SongWithScoreModel,string> predicate = OrderedBy switch
    {
      SortingType.Id => m => $"{m.Song.Id}",
      SortingType.Artist => m => m.Song.Artist,
      SortingType.Title => m => m.Song.Title,
      SortingType.Score => m => $"{m.Score?.Score ?? 0}",
      SortingType.Player => m => m.Score?.Player ?? string.Empty,
      SortingType.Date => m => $"{m.Score?.Date ?? 0}",
      _ => throw new ArgumentOutOfRangeException()
    };
    
    SongsWithScoresOrdered = Ascending
      ? SongsWithScores.OrderBy(predicate).ToList()
      : SongsWithScores.OrderByDescending(predicate).ToList();
    
    OnPropertyChanged(nameof(SongsWithScoresOrdered));
  }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}