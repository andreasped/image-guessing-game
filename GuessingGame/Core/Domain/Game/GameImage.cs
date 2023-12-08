namespace GuessingGame.Core.Domain.Game
{
    public class GameImage
    {
        public GameImage() { }
        public GameImage(string imgSource, string label, List<GameTile> tiles, int imageId)
        {
            ImgSource = imgSource;
            Label = label;
            Tiles = tiles;
            ImageId = imageId;

        }

        public int Id { get; set; }
        public int ImageId { get; set; }
        public string ImgSource { get; set; }
        public string Label { get; set; }
        public List<GameTile> Tiles { get; set; }
    }
}