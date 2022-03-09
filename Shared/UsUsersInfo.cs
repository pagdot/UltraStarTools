namespace Shared
{
    public partial class UsUsersInfo
    {
        public long WebId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public long SendSavePlayer { get; set; }
        public long AutoMode { get; set; }
        public long AutoPlayer { get; set; }
        public long AutoScoreEasy { get; set; }
        public long AutoScoreMedium { get; set; }
        public long AutoScoreHard { get; set; }
    }
}
