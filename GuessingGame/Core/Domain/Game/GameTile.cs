namespace GuessingGame.Core.Domain.Game
{
    public class GameTile
    {
        public GameTile(string source)
        {
            Source = source;
        }
        public int Id { get; set; }
        public string Source { get; set; }
    }
}