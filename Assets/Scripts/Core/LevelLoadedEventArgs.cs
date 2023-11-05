namespace TankGame.Core
{
    /// <summary>
    /// Contains the relevant data for the level loaded event.
    /// </summary>
    public readonly struct LevelLoadedEventArgs
    {
        /// <summary>
        /// Represents the name of the level that was loaded.
        /// </summary>
        public string LevelName { get; }

        /// <summary>
        /// Creates a new instance with the level name.
        /// </summary>
        /// <param name="levelName">name of the level</param>
        public LevelLoadedEventArgs(string levelName)
        {
            LevelName = levelName;
        }
    }
}