using System.Collections.Generic;

namespace Assets.Logic
{
    internal class LevelStepResult
    {
        public bool HasPlayerMoved { get; set; }
        public IList<GameEntity> RemovedEntities { get; set; }
        public bool IsGameLost { get; set; } = false;
        public bool IsGameWon { get; set; } = false;
    }
}
