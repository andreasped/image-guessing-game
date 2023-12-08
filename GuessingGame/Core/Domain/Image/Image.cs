using GuessingGame.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace GuessingGame.Core.Domain.Image
{
    public class Image : BaseEntity
    {
        public Image() { }

        public Image(string imgSource, List<Tile> tile, string label)
        {
            ImgSource = imgSource;
            Tile = tile;
            Label = label;
        }

        public int Id { get; set; }
        public string ImgSource { get; set; }
        public string Label { get; set; }
        public List<UserImprovedTileOrder> UserImprovedTileOrder { get; set; } = new();
        public List<Tile> Tile { get; set; }

        public void AddToTemporaryTileList(int tileId)
        {
            if (UserImprovedTileOrder == null)
            {
                UserImprovedTileOrder = new List<UserImprovedTileOrder>();
            }
            UserImprovedTileOrder.Add(new UserImprovedTileOrder(tileId));
        }
    }
}