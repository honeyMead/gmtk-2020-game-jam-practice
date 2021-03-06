using System.Collections.Generic;
using System.Linq;

namespace Assets.Logic
{
    internal class LevelStepResult
    {
        public bool HasPlayerMoved { get; set; } = false;
        public bool HasSomethingChanged { get; set; } = false;
        public IEnumerable<GameEntity> RemovedEntities { get; set; } = Enumerable.Empty<GameEntity>();
        public bool IsGameLost { get; set; } = false;
        public bool IsGameWon { get; set; } = false;
    }
}
