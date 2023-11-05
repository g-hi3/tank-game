using System;

namespace TankGame.Core.Save
{
    /// <summary>
    /// Represents the serializable level time.
    /// </summary>
    [Serializable]
    public class LevelTime
    {
        /// <summary>
        /// Name of the level.
        /// </summary>
        public string name;

        /// <summary>
        /// Time the player needed to beat this level and all before in this attempt.
        /// </summary>
        public TimeSpan time;

        /// <summary>
        /// Necessary constructor for Unity's serialization.
        /// </summary>
        /// <remarks>
        /// Always use <see cref="LevelTime(string, TimeSpan)"/> instead!
        /// </remarks>
        public LevelTime()
        {
        }

        /// <summary>
        /// Creates a new level time.
        /// </summary>
        /// <param name="name">name of the level</param>
        /// <param name="time">cumulative time of this level and the ones before of this attempt</param>
        public LevelTime(string name, TimeSpan time)
        {
            this.name = name;
            this.time = time;
        }
    }
}