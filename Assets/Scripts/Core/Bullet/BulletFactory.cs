using UnityEngine;

namespace TankGame.Core.Bullet
{
    public class BulletFactory : MonoBehaviour
    {
        [field: SerializeField] public BulletBlueprint Blueprint { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }

        public GameObject Make(Transform spawn)
        {
            var bulletDirection = spawn.position - transform.position;
            var bulletObject = Instantiate(Prefab, spawn.position, spawn.rotation);
            _ = Bullet.FromBlueprint(Blueprint, bulletDirection, bulletObject);
            return bulletObject;
        }
    }
}