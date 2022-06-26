namespace Assets.Logic
{
    internal class Level
    {
        private readonly GameEntity[,] Tiles;

        public Level(int horizontalTiles, int verticalTiles)
        {
            Tiles = new GameEntity[horizontalTiles, verticalTiles];
        }
    }
}
