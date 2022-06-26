using System.Collections.Generic;
using System.Linq;

namespace Assets.Logic
{
    internal class Level
    {
        public const string PlayerName = "Player";
        public const string ChickenName = "Chicken";
        public const string StrawBaleName = "StrawBale";
        public const string ExitName = "Exit";

        public readonly int xSize;
        public readonly int ySize;

        private readonly GameEntity[,] Tiles;
        private GameEntity player;
        private IList<GameEntity> chicken = new List<GameEntity>();
        private IList<GameEntity> strawBales = new List<GameEntity>();
        private GameEntity exit;
        private IList<GameEntity> allEntities = new List<GameEntity>();

        public Level(int horizontalTiles, int verticalTiles)
        {
            xSize = horizontalTiles;
            ySize = verticalTiles;
            Tiles = new GameEntity[horizontalTiles, verticalTiles];
        }

        /// <returns>If player has moved.</returns>
        public bool Next(Position direction)
        {
            var hasPlayerMoved = MovePlayer(direction);

            // TODO remove burned down stuff

            InflameEntities();

            // TODO move normal chickens
            return hasPlayerMoved;
        }

        private void InflameEntities()
        {
            var entitiesToInflame = new List<GameEntity>();
            foreach (var entity in allEntities)
            {
                if (entity.IsFlammable && !entity.IsBurning)
                {
                    var neighbors = GetNeighborEntities(entity);
                    var isNextToFire = neighbors.Any(n => n.IsBurning);

                    if (isNextToFire)
                    {
                        entitiesToInflame.Add(entity);
                    }
                }
            }
            entitiesToInflame.ForEach(a => a.IsBurning = true);
        }

        private bool MovePlayer(Position direction)
        {
            var hasPlayerMoved = false;
            var target = new Position(player.Position.X + direction.X, player.Position.Y + direction.Y);
            if (IsInsideBounds(target))
            {
                var entityAtTarget = GetEntity(target.X, target.Y);

                if (entityAtTarget == null)
                {
                    SetEntity(player.Position.X, player.Position.Y, null);
                    player.Position = target;
                    SetEntity(target.X, target.Y, player);
                    hasPlayerMoved = true;
                }
            }
            return hasPlayerMoved;
        }

        public GameEntity GetEntity(int xIndex, int yIndex)
        {
            return Tiles[xIndex, yIndex];
        }

        public void PlaceEntity(int xIndex, int yIndex, GameEntity gameEntity)
        {
            switch (gameEntity.Name)
            {
                case PlayerName:
                    player = gameEntity;
                    break;
                case ChickenName:
                    chicken.Add(gameEntity);
                    break;
                case StrawBaleName:
                    strawBales.Add(gameEntity);
                    break;
                case ExitName:
                    exit = gameEntity;
                    break;
            }
            // TODO remove previous entities
            allEntities.Add(gameEntity);

            gameEntity.Position.X = xIndex;
            gameEntity.Position.Y = yIndex;

            SetEntity(xIndex, yIndex, gameEntity);
        }

        private IList<GameEntity> GetNeighborEntities(GameEntity originEntity)
        {
            var neighborEntities = new List<GameEntity>();
            var x = originEntity.Position.X;
            var y = originEntity.Position.Y;

            var neighborPositions = new Position[]
            {
                new Position(x - 1, y),
                new Position(x + 1, y),
                new Position(x, y - 1),
                new Position(x, y + 1),
            };
            foreach (var neighborPosition in neighborPositions)
            {
                if (IsInsideBounds(neighborPosition))
                {
                    var neighbor = GetEntity(neighborPosition.X, neighborPosition.Y);
                    if (neighbor != null)
                    {
                        neighborEntities.Add(neighbor);
                    }
                }
            }
            return neighborEntities;
        }

        private void SetEntity(int xIndex, int yIndex, GameEntity gameEntity)
        {
            Tiles[xIndex, yIndex] = gameEntity;
        }

        private bool IsInsideBounds(Position target)
        {
            return target.X >= 0 && target.Y >= 0
                && target.X <= Tiles.GetUpperBound(0) && target.Y <= Tiles.GetUpperBound(1);
        }

        public void PrintTiles()
        {
            //for (int y = Tiles.GetLength(1) - 1; y >= 0; y--)
            //{
            //    var row = "";
            //    for (int x = 0; x < Tiles.GetLength(0); x++)
            //    {
            //        {
            //            var text = Tiles[x, y]?.ToString() ?? " . ";
            //            row += text + "    ";
            //        }
            //    }
            //    Debug.Log(row);
            //}
        }
    }
}
