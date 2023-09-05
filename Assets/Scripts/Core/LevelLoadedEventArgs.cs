namespace TankGame.Core
{
    public struct LevelLoadedEventArgs
    {
        public string LevelName { get; }

        public LevelLoadedEventArgs(string levelName)
        {
            LevelName = levelName;
        }
    }
}