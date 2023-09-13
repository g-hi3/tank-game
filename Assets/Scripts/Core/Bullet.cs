﻿using UnityEngine;

namespace TankGame.Core
{
    public class Bullet : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        private uint _remainingRicochetCount;
        private Vector2 _velocity;

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
            var rotationZ = 90f - Mathf.Atan2(_velocity.x, _velocity.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, rotationZ);
        }

        private void Update()
        {
            if (_remainingRicochetCount < 1)
                Destroy(gameObject);
            
            transform.position += (Vector3)_velocity * Time.deltaTime;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // TODO: This is called twice, the gameObject of "other" is Tank(Clone) both times!
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

        public static Bullet FromBlueprint(BulletBlueprint blueprint, Vector2 bulletRotation, Transform spawn)
        {
            var gameObject = Instantiate(blueprint.Prefab, spawn.position, spawn.rotation);
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
