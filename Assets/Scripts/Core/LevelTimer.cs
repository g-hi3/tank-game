using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankGame.Core
{
    /// <summary>
    /// This component records the time elapsed per level.
    /// </summary>
    public class LevelTimer : MonoBehaviour
    {
        private int _totalMilliseconds;
        private readonly Stack<int> _steps = new();

        /// <summary>
        /// Determines if time is recorded or not.
        /// </summary>
        public bool Paused { get; private set; } = true;

        /// <summary>
        /// Calculates the millisecond component for display.
        /// </summary>
        public int Milliseconds => _totalMilliseconds % 1000;

        /// <summary>
        /// Calculates the second component for display.
        /// </summary>
        public int Seconds => _totalMilliseconds / 1000 % 60;

        /// <summary>
        /// Calculates the minute component for display.
        /// </summary>
        public int Minutes => _totalMilliseconds / 60_000;

        /// <summary>
        /// This method creates an array of <see cref="TimeSpan"/> that represent the steps.
        /// </summary>
        /// <remarks>
        /// A step is when the player won that level.
        /// </remarks>
        /// <returns>all timer steps</returns>
        public TimeSpan[] GetSteps()
        {
            if (_steps == null)
                return Array.Empty<TimeSpan>();

            return _steps
                .Select(milliseconds => TimeSpan.FromMilliseconds(milliseconds))
                .ToArray();
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            Paused = true;
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            Paused = false;
        }

        /// <summary>
        /// Creates a new step.
        /// </summary>
        /// <remarks>
        /// This operation does not pause or resume the timer.
        /// </remarks>
        public void CreateStep()
        {
            _steps?.Push(_totalMilliseconds);
        }

        /// <summary>
        /// Sets the timer to the most recent step, or zero.
        /// </summary>
        public void ResetStep()
        {
            _totalMilliseconds = _steps?.Count > 0
                ? _steps.Peek()
                : 0;
        }

        private void Update()
        {
            if (!Paused)
                _totalMilliseconds += (int)(Time.deltaTime * 1000f);
        }
    }
}
