using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankGame.Core
{
    public class LevelTimer : MonoBehaviour
    {
        private int _totalMilliseconds;
        private readonly Stack<int> _steps = new();

        public bool Paused { get; private set; } = true;
        public int Milliseconds => _totalMilliseconds % 1000;
        public int Seconds => _totalMilliseconds / 1000 % 60;
        public int Minutes => _totalMilliseconds / 60_000;

        public TimeSpan[] GetSteps()
        {
            return _steps
                .Select(milliseconds => TimeSpan.FromMilliseconds(milliseconds))
                .ToArray();
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void ResetTime()
        {
            _totalMilliseconds = 0;
        }

        public void CreateStep()
        {
            _steps.Push(_totalMilliseconds);
        }

        public void ResetStep()
        {
            _totalMilliseconds = _steps.Count > 0
                ? _steps.Peek()
                : 0;
        }

        public void ClearSteps()
        {
            _steps.Clear();
        }

        private void Update()
        {
            if (!Paused)
                _totalMilliseconds += (int)(Time.deltaTime * 1000f);
        }
    }
}
