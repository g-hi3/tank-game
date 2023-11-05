using System;
using JetBrains.Annotations;
using TankGame.Core.Bomb;
using UnityEngine;

namespace TankGame.Core.Bullet
{
    /// <summary>
    /// This component moves in a straight line, reflects from walls and can hit game objects with a
    /// <see cref="IBulletTarget"/> component.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class Bullet : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        private const float BulletBaseOrientationDegrees = 90f;
        [NotNull] private Transform _transform = default!;
        private uint _remainingRicochetCount;
        private Vector2 _velocity;
        private bool _paused;

        /// <summary>
        /// Destroys this game object when hit by an explosion.
        /// </summary>
        public void OnDetonationHit() => Destroy(gameObject);

        /// <summary>
        /// Destroys this game object when hit by another bullet.
        /// </summary>
        public void OnBulletHit() => Destroy(gameObject);

        private void Hit(IBulletTarget bulletTarget)
        {
            bulletTarget?.OnBulletHit();
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
            _transform.eulerAngles = new Vector3(0f, 0f, rotationZ);
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
        }

        private void Update()
        {
            if (_paused)
                return;
            
            if (_remainingRicochetCount < 1)
                Destroy(gameObject);
            
            _transform.position += (Vector3)_velocity * Time.deltaTime;
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
            if (other == null || other.gameObject == null)
                return;
            if (other.gameObject.TryGetComponent(out IBulletTarget bulletTarget))
                Hit(bulletTarget);
            else if (other.contacts is { Length: >0 })
                ReflectFrom(other.contacts[0]);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other && other.gameObject.TryGetComponent(out IBulletTarget bulletTarget))
                Hit(bulletTarget);
        }

        /// <summary>
        /// Creates a new instance using a blueprint.
        /// </summary>
        /// <remarks>
        /// The magnitude of <paramref name="bulletRotation"/> has no effect on the speed of the bullet. Additionally,
        /// this method won't create a new component, if the given <paramref name="gameObject"/> already
        /// has one.
        /// </remarks>
        /// <param name="blueprint">blueprint containing data for this bullet</param>
        /// <param name="bulletRotation">rotation of this bullet; a 2D direction</param>
        /// <param name="gameObject">the game object this component should be attached to</param>
        /// <returns>the created component</returns>
        public static Bullet FromBlueprint(
            [NotNull] BulletBlueprint blueprint,
            Vector2 bulletRotation,
            [NotNull] GameObject gameObject)
        {
            var bullet = GetOrAddBullet(gameObject);
            bullet._remainingRicochetCount = blueprint.RicochetCount;
            bullet._velocity = bulletRotation.normalized * blueprint.Speed;
            return bullet;
        }

        [NotNull]
        private static Bullet GetOrAddBullet([NotNull] GameObject gameObject)
        {
            return gameObject.TryGetComponent<Bullet>(out var component)
                ? component!
                : gameObject.AddComponent<Bullet>()!;
        }
    }
}
