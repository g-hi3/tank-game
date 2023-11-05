using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Component that moves an enemy tank.
    /// </summary>
    public abstract class Mover : MonoBehaviour
    {
        /// <summary>
        /// Moves the enemy tank.
        /// </summary>
        /// <remarks>
        /// This method is expected to be called once per frame per tank.
        /// </remarks>
        public abstract void Move();
    }
}