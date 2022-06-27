namespace Assets.Logic
{
    internal class GameEntity
    {
        public string Name { get; set; }
        public Position Position { get; set; } = new Position();
        public bool IsFlammable { get; set; } = false;
        public bool IsBurning { get; set; } = false;

        public bool HasMaxRoundsNextToFire { get; set; } = false;
        public int RoundsNextToFire { get; set; } = 0;
        public int MaxRoundsNextToFire { get; set; } = 0;

        public override string ToString()
        {
            return Name.Substring(0, 1);
        }

        public bool IsEmpty()
        {
            return Name == Level.EmptyName;
        }
    }
}
