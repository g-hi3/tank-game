using JetBrains.Annotations;
using TankGame.Core.Bomb;
using UnityEngine;

namespace TankGame.Core.Bullet
{
    public class Bullet : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        private const float BulletBaseOrientationDegrees = 90f;
        private uint _remainingRicochetCount;
        private Vector2 _velocity;
        private bool _paused;

        public void OnDetonationHit() => Destroy(gameObject);

        public void OnBulletHit() => Destroy(gameObject);

        private void Hit(IBulletTarget bulletTarget)
        {
            bulletTarget.OnBulletHit();
            Destroy(gameObject);
        }

        private void ReflectFrom(ContactPoint2D contactPoint)
        {
            _velocity = Vector2.Reflect(_velocity, contactPoint.normal);
            _remainingRicochetCount--;
            RotateUsingVelocity();
        }

        private void RotateUsingVelocity()
        {
            var rotationZ = BulletBaseOrientationDegrees - Mathf.Atan2(_velocity.x, _velocity.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, rotationZ);
        }

        private void Update()
        {
            if (_paused)
                return;
            
            if (_remainingRicochetCount < 1)
                Destroy(gameObject);
            
            transform.position += (Vector3)_velocity * Time.deltaTime;
        }

        [UsedImplicitly]
        private void OnPause()
        {
            _paused = true;
        }

        [UsedImplicitly]
        private void OnResume()
        {
            _paused = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out IBulletTarget bulletTarget))
                Hit(bulletTarget);
            else
                ReflectFrom(other.contacts[0]);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out IBulletTarget bulletTarget))
                Hit(bulletTarget);
        }

        public static Bullet FromBlueprint(BulletBlueprint blueprint, Vector2 bulletRotation, GameObject gameObject)
        {
            var bullet = GetOrAddBullet(gameObject);
            bullet._remainingRicochetCount = blueprint.RicochetCount;
            bullet._velocity = bulletRotation.normalized * blueprint.Speed;
            return bullet;
        }

        private static Bullet GetOrAddBullet(GameObject gameObject)
        {
            return gameObject.TryGetComponent<Bullet>(out var component)
                ? component
                : gameObject.AddComponent<Bullet>();
        }
    }
}
