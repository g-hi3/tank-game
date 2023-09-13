using UnityEngine;

namespace TankGame.Core
{
    public class BulletFactory : MonoBehaviour
    {
        [field: SerializeField] public BulletBlueprint Blueprint { get; private set; }
        [field: SerializeField] public ObjectSpawner Spawner { get; private set; }
        [field: SerializeField] public Transform Spawn { get; private set; }

        public Bullet Make()
        {
            var bulletDirection = Spawn.position - transform.position;
            return Bullet.FromBlueprint(Blueprint, bulletDirection, Spawn);
        }
    }
}
