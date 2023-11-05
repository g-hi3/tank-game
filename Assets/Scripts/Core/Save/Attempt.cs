using System;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Core.Save
{
    /// <summary>
    /// Represents an attempt at the game with recorded times for each level.
    /// </summary>
    [Serializable]
    public class Attempt
    {
        /// <summary>
        /// Contains the times for all levels.
        /// </summary>
        public LevelTime[] levelTimes;

        /// <summary>
        /// Creates a new instance without level times.
        /// </summary>
        /// <remarks>
        /// This constructor is necessary for Unity's serialization. Always call
        /// <see cref="Attempt(IEnumerable{LevelTime})"/> instead!
        /// </remarks>
        public Attempt() : this(Array.Empty<LevelTime>())
        {
        }

        /// <summary>
        /// Creates a new instance with the given level times.
        /// </summary>
        /// <param name="levelTimes">level times; levels should be distinct and times are cumulative</param>
        public Attempt(IEnumerable<LevelTime> levelTimes)
        {
            this.levelTimes = levelTimes?.ToArray() ?? Array.Empty<LevelTime>();
        }
    }
}