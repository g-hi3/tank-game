using System;
using JetBrains.Annotations;
using TankGame.Core.Bomb;
using TankGame.Core.Bullet;
using UnityEngine;

namespace TankGame.Core
{
    /// <summary>
    /// This component provides all basic control methods for a tank.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Transform))]
    public class TankController : MonoBehaviour
    {
        private const float RotationZOffset = 270f;
        private static readonly int MoveSpeedAnimatorProperty = Animator.StringToHash("Move Speed");
        [SerializeField] private Transform head;
        [SerializeField] private float moveSpeed;
        [NotNull] private Transform _transform = default!;
        [NotNull] private Rigidbody2D _rigidbody = default!;
        [NotNull] private Animator _animator = default!;
        private bool _isMoving;
        private Vector3 _moveDirection;
        private Quaternion _moveRotation;
        private Quaternion _lookRotation;

        /// <summary>
        /// This component spawns bullets for this tank.
        /// </summary>
        [field: SerializeField] public BulletSpawner BulletSpawner { get; private set; }

        /// <summary>
        /// This component spawns bombs for this tank.
        /// </summary>
        [field: SerializeField] public BombSpawner BombSpawner { get; private set; }

        private void OnExplosionEnded()
        {
            Destroy(gameObject);
        }
  
        /// <summary>
        /// Starts the moving animation and sets the moving state.
        /// </summary>
        public void StartMoving()
        {
            _isMoving = true;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 1f);
        }

        /// <summary>
        /// Moves the tank into the given <paramref name="direction"/>.
        /// </summary>
        /// <remarks>
        /// Calling this method does not start the moving animation or set the moving state. To do this, call
        /// <see cref="StartMoving"/>.
        /// </remarks>
        /// <param name="direction">the direction in which to move</param>
        public void Move(Vector3 direction)
        {
            _moveDirection = moveSpeed * direction;
            _moveRotation = GetRotation2DInDirection(direction);
        }

        private static Quaternion GetRotation2DInDirection(Vector3 direction)
        {
            var rotationZ = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - RotationZOffset;
            return Quaternion.Euler(0f, 0f, rotationZ);
        }

        /// <summary>
        /// Stops the moving animation and unsets the moving state.
        /// </summary>
        public void StopMoving()
        {
            _isMoving = false;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 0f);
        }

        /// <summary>
        /// Rotates the tank's aim to the given <paramref name="direction"/>
        /// </summary>
        /// <param name="direction">direction to look</param>
        public void Look(Vector2 direction)
        {
            _lookRotation = GetRotation2DInDirection(direction);
        }

        /// <summary>
        /// Rotates to tank's aim to look at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">world position at which the tank should look</param>
        public void LookAt(Vector3 position)
        {
            var direction = position - _transform.position;
            Look(direction);
        }

        /// <summary>
        /// Tries to spawn a bullet.
        /// </summary>
        public void Shoot()
        {
            if (BulletSpawner)
                BulletSpawner.TrySpawn();
        }

        /// <summary>
        /// Tries to spawn a bomb.
        /// </summary>
        public void Bomb()
        {
            if (BombSpawner)
                _ = BombSpawner.TrySpawn();
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
            _rigidbody = GetComponent<Rigidbody2D>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Rigidbody2D)} component!");
            _animator = GetComponent<Animator>()
                        ?? throw new InvalidOperationException($"Missing {nameof(Animator)} component!");
        }

        private void Start()
        {
            _lookRotation = _transform.rotation;
        }

        private void Update()
        {
            _transform.rotation = _moveRotation;

            if (head)
                head.rotation = _lookRotation;
        }

        private void FixedUpdate()
        {
            if (_isMoving)
                _rigidbody.velocity = _moveDirection;
        }
    }
}
