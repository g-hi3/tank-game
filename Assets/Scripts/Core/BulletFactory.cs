using UnityEngine;

namespace TankGame.Core
{
    public class BulletFactory : MonoBehaviour
    {
        public Bullet Make(BulletBlueprint blueprint)
        {
            var bulletRotation = transform.eulerAngles;
            return Bullet.FromBlueprint(blueprint, bulletRotation);
        }
    }
}