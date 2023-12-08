namespace GuessingGame.Core.Domain.Image
{
    public class UserImprovedTileOrder
    {
        public UserImprovedTileOrder(int tileId)
        {
            TileId = tileId;
        }
        public int Id { get; set; }
        public int TileId { get; set; }

    }
}