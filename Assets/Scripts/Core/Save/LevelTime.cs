using System;

namespace TankGame.Core.Save
{
    [Serializable]
    public class LevelTime
    {
        public LevelTime()
        {
        }

        public LevelTime(string name, TimeSpan time)
        {
            Name = name;
            Time = time;
        }

        public string Name { get; set; }
        public TimeSpan Time { get; set; }
    }
}