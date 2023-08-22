using UnityEngine;

namespace TankGame.Util
{
    public class FrameRateRegulator : MonoBehaviour
    {
        [field: SerializeField]
        [field: Min(0)] 
        public int TargetFrameRate { get; private set; }
  
        private void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
