using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Logic
{
    internal class Level
    {
        public const string PlayerName = "Player";
        public const string ChickenName = "Chicken";
        public const string StrawBaleName = "StrawBale";
        public const string EmptyName = "_Empty";

        public readonly int xSize;
        public readonly int ySize;

        private readonly GameEntity[,] Tiles;
        private GameEntity player;
        private Position exitPosition;
        private readonly IList<GameEntity> allEntities = new List<GameEntity>();

        public Level(int horizontalTiles, int verticalTiles)
        {
            xSize = horizontalTiles;
            ySize = verticalTiles;
            Tiles = new GameEntity[horizontalTiles, verticalTiles];
            InitEmptySpace();
        }

        private void InitEmptySpace()
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    SetEmptyAtPosition(x, y);
                }
            }
        }

        public IEnumerable<LevelStepResult> Next(Position direction)
        {
            var hasPlayerMoved = MovePlayer(direction);
            var result = new LevelStepResult
            {
                HasPlayerMoved = hasPlayerMoved,
                HasSomethingChanged = hasPlayerMoved,
            };
            yield return result;

            if (result.HasPlayerMoved)
            {
                // TODO check if game won: all chickens saved and exit reached
                result.HasSomethingChanged = InflameEntities();
                yield return result;
                result.HasSomethingChanged = MoveChicken();
                yield return result;

                var fledChicken = RemoveFledChicken();
                var burnedDownStuff = RemoveBurnedDownStuff();
                if (burnedDownStuff.Any(r => r.Name == ChickenName))
                {
                    result.IsGameLost = true;
                }
                result.RemovedEntities = burnedDownStuff.Concat(fledChicken);
                result.HasSomethingChanged = result.RemovedEntities.Count() > 0;
                yield return result;
            }
        }

        private bool MoveChicken()
        {
            var hasAChickenMoved = false;
            var chicken = allEntities.Where(n => n.Name == ChickenName);

            foreach (var chick in chicken)
            {
                var neighbors = GetNeighborEntities(chick);
                var emptyNeighbors = neighbors.Where(n => n.IsEmpty());
                var burningNeighbors = neighbors.Where(n => n.IsBurning);

                if (burningNeighbors.Any() && emptyNeighbors.Any())
                {
                    var bestPosition = emptyNeighbors.First().Position;

                    var isNextToExit = emptyNeighbors.Any(entity => entity.Position.Equals(exitPosition));
                    if (isNextToExit)
                    {
                        bestPosition = exitPosition;
                    }
                    else
                    {
                        foreach (var emptyNeighbor in emptyNeighbors)
                        {
                            var nextNeighbors = GetNeighborEntities(emptyNeighbor);
                            var hasNoBurningNeighbors = nextNeighbors.All(n => !n.IsBurning);
                            if (hasNoBurningNeighbors)
                            {
                                bestPosition = emptyNeighbor.Position;
                                break;
                            }
                        }
                    }
                    MoveEntityToPosition(chick, bestPosition);
                    hasAChickenMoved = true;
                }
            }
            return hasAChickenMoved;
        }

        private bool InflameEntities()
        {
            var entitiesToInflame = new List<GameEntity>();
            foreach (var entity in allEntities)
            {
                if (entity.IsFlammable && !entity.IsBurning)
                {
                    if (IsNextToFire(entity))
                    {
                        entitiesToInflame.Add(entity);
                    }
                }
            }
            entitiesToInflame.ForEach(a => a.IsBurning = true);
            return entitiesToInflame.Count() > 0;
        }

        private IList<GameEntity> RemoveBurnedDownStuff()
        {
            var entitiesToRemove = new List<GameEntity>();
            foreach (var entity in allEntities)
            {
                if (entity.HasMaxRoundsNextToFire)
                {
                    if (IsNextToFire(entity))
                    {
                        entity.RoundsNextToFire += 1;

                        if (entity.RoundsNextToFire >= entity.MaxRoundsNextToFire)
                        {
                            entitiesToRemove.Add(entity);
                            SetEmptyAtPosition(entity.Position);
                        }
                    }
                    else
                    {
                        entity.RoundsNextToFire = 0;
                    }
                }
            }
            entitiesToRemove.ForEach(r => allEntities.Remove(r));
            return entitiesToRemove;
        }

        private IList<GameEntity> RemoveFledChicken()
        {
            var entitiesToRemove = new List<GameEntity>();
            var chicken = allEntities.Where(n => n.Name == ChickenName);

            foreach (var chick in chicken)
            {
                if (chick.Position.Equals(exitPosition))
                {
                    entitiesToRemove.Add(chick);
                    chick.IsBurning = false;
                    SetEmptyAtPosition(chick.Position);
                }
            }
            entitiesToRemove.ForEach(r => allEntities.Remove(r));
            return entitiesToRemove;
        }

        private bool IsNextToFire(GameEntity entity)
        {
            var neighbors = GetNeighborEntities(entity);
            var isNextToFire = neighbors.Any(n => n.IsBurning);
            return isNextToFire;
        }

        private bool MovePlayer(Position direction)
        {
            var hasPlayerMoved = false;
            var target = new Position(player.Position.X + direction.X, player.Position.Y + direction.Y);
            if (IsInsideBounds(target))
            {
                var entityAtTarget = GetEntity(target.X, target.Y);

                if (entityAtTarget.IsEmpty())
                {
                    MoveEntityToPosition(player, target);
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
                    player.IsBurning = true;
                    break;
                case ChickenName:
                    gameEntity.HasMaxRoundsNextToFire = true;
                    gameEntity.MaxRoundsNextToFire = 2;
                    break;
                case StrawBaleName:
                    gameEntity.IsFlammable = true;
                    break;
            }
            var prevEntity = allEntities.FirstOrDefault(n => n.Position.X == xIndex && n.Position.Y == yIndex);
            if (prevEntity != null)
            {
                allEntities.Remove(prevEntity);
            }
            allEntities.Add(gameEntity);

            gameEntity.Position.X = xIndex;
            gameEntity.Position.Y = yIndex;

            SetEntity(xIndex, yIndex, gameEntity);
        }

        public void SetExitPosition(int x, int y)
        {
            exitPosition = new Position(x, y);
        }

        private void MoveEntityToPosition(GameEntity entity, Position target)
        {
            SetEmptyAtPosition(entity.Position);
            entity.Position = target;
            SetEntity(target.X, target.Y, entity);
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
                    neighborEntities.Add(neighbor);
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

        private void SetEmptyAtPosition(Position pos)
        {
            SetEmptyAtPosition(pos.X, pos.Y);
        }

        private void SetEmptyAtPosition(int x, int y)
        {
            SetEntity(x, y, new GameEntity()
            {
                Name = EmptyName,
                Position = new Position(x, y),
            });
        }

        public void PrintTiles()
        {
            for (int y = Tiles.GetLength(1) - 1; y >= 0; y--)
            {
                var row = "";
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    {
                        var entity = Tiles[x, y];
                        var entityText = entity.ToString();
                        if (entity.IsEmpty() && x == exitPosition.X && y == exitPosition.Y)
                        {
                            entityText = "X";
                        }
                        row += entityText + "    ";
                    }
                }
                Debug.Log(row);
            }
            Debug.Log("------------------");
        }
    }
}
