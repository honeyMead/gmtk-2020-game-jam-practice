namespace Assets.Logic
{
    internal class GameEntity
    {
        public string Name { get; set; }
        public Position Position { get; set; } = new Position();
        public bool IsFlammable { get; set; } = false;
        public bool IsBurning { get; set; } = false;

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
