using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Logic
{
    internal class Level
    {
        public const string PlayerName = "Player";
        public const string ChickenName = "Chicken";
        public const string StrawBaleName = "StrawBale";
        public const string ExitName = "Exit";

        private readonly GameEntity[,] Tiles;
        private GameEntity player;
        private IList<GameEntity> chicken = new List<GameEntity>();
        private IList<GameEntity> strawBales = new List<GameEntity>();
        private GameEntity exit;

        public Level(int horizontalTiles, int verticalTiles)
        {
            Tiles = new GameEntity[horizontalTiles, verticalTiles];
        }

        /// <returns>If player has moved.</returns>
        public bool Next(Position direction)
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

            // TODO remove burned down stuff
            // TODO move fires
            // TODO move normal chickens
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
            gameEntity.Position.X = xIndex;
            gameEntity.Position.Y = yIndex;
            SetEntity(xIndex, yIndex, gameEntity);
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
            for (int y = Tiles.GetLength(1) - 1; y >= 0; y--)
            {
                var row = "";
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    {
                        var text = Tiles[x, y]?.ToString() ?? " . ";
                        row += text + "    ";
                    }
                }
                Debug.Log(row);
            }
        }
    }
}
