using UnityEngine;

namespace TankGame.Util
{
    /// <summary>
    /// This component sets the target frame rate when the level starts.
    /// </summary>
    public class FrameRateRegulator : MonoBehaviour
    {
        /// <summary>
        /// Represents the target frame rate.
        /// </summary>
        /// <seealso cref="Application.targetFrameRate">Application.targetFrameRate</seealso>
        [field: SerializeField]
        [field: Min(0)] 
        public int TargetFrameRate { get; private set; }
  
        private void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
