namespace GuessingGame.Core.Domain.Image
{
    public class Tile
    {
        public Tile(string source)
        {
            Source = source;
        }
        public int Id { get; set; }
        public string Source { get; set; }
    }
}