using TankGame.Core.Bomb;
using TankGame.Core.Bullet;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Core
{
    /// <summary>
    /// This component contains tank elimination logic.
    /// </summary>
    public class Tank : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        /// <summary>
        /// This event is fired when the tank was eliminated.
        /// </summary>
        [field: SerializeField] public UnityEvent<Tank> Eliminated { get; private set; }

        /// <summary>
        /// This method eliminates the tank.
        /// </summary>
        public void OnDetonationHit() => Eliminate();

        /// <summary>
        /// This method eliminates the tank.
        /// </summary>
        public void OnBulletHit() => Eliminate();

        private void Eliminate()
        {
            if (!GameManager.Instance.GameRulesActive)
                return;

            Destroy(gameObject);
            Eliminated?.Invoke(this);
        }
    }
}
