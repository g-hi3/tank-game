using TankGame.Core.Bomb;
using TankGame.Core.Bullet;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Core
{
    public class Tank : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        [field: SerializeField] public UnityEvent<Tank> Eliminated { get; private set; }

        public void OnDetonationHit() => Eliminate();

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
