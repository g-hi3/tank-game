using UnityEngine;

namespace TankGame.Core
{
    public class BulletFactory : MonoBehaviour
    {
        [field: SerializeField] public BulletBlueprint Blueprint { get; private set; }

        public Bullet Make()
        {
            var bulletRotation = transform.eulerAngles;
            return Bullet.FromBlueprint(Blueprint, bulletRotation);
        }
    }
}