using UnityEngine;

namespace TankGame.Core
{
    public class LevelTimer : MonoBehaviour
    {
        private int _totalMilliseconds;

        public int Milliseconds => _totalMilliseconds % 1000;
        public int Seconds => _totalMilliseconds / 1000 % 60;
        public int Minutes => _totalMilliseconds / 60_000;

        private void Update()
        {
            _totalMilliseconds = (int)(Time.time * 1000f);
        }
    }
}
