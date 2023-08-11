using UnityEngine;

namespace TankGame.Core
{
    public class Tank : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        public void OnDetonationHit() => Destroy(gameObject);

        public void OnBulletHit() => Destroy(gameObject);
    }
}
