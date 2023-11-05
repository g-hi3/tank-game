using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Controls the aim direction of an enemy tank.
    /// </summary>
    public abstract class Rotator : MonoBehaviour
    {
        /// <summary>
        /// Rotates the aim direction of a tank.
        /// </summary>
        public abstract void Rotate();
    }
}