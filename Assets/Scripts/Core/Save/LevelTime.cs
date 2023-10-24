using System;

namespace TankGame.Core.Save
{
    [Serializable]
    public class LevelTime
    {
        public string Name;
        public TimeSpan Time;

        public LevelTime()
        {
        }

        public LevelTime(string name, TimeSpan time)
        {
            Name = name;
            Time = time;
        }
    }
}