namespace ScanSongLibrary;

public static class DateTimeExtensions
{
    public static long ToUnix(this DateTime dateTime)
    {
        return dateTime.Subtract(DateTime.UnixEpoch.ToLocalTime()).Seconds;
    }
    
    public static DateTime FromUnix(long sec)
    {
        // Unix timestamp is seconds past epoch
        var dateTime = DateTime.UnixEpoch;
        dateTime = dateTime.AddSeconds(sec);
        return dateTime;
    }
}