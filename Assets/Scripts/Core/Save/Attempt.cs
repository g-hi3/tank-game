using System;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Core.Save
{
    [Serializable]
    public class Attempt
    {
        public LevelTime[] LevelTimes;

        public Attempt() : this(Array.Empty<LevelTime>())
        {
        }

        public Attempt(IEnumerable<LevelTime> levelTimes)
        {
            LevelTimes = levelTimes?.ToArray() ?? Array.Empty<LevelTime>();
        }
    }
}