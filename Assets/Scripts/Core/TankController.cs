using System;
using JetBrains.Annotations;
using TankGame.Core.Bomb;
using TankGame.Core.Bullet;
using UnityEngine;

namespace TankGame.Core
{
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
        
        [field: SerializeField] public BulletSpawner BulletSpawner { get; private set; }
        [field: SerializeField] public BombSpawner BombSpawner { get; private set; }

        private void OnExplosionEnded()
        {
            Destroy(gameObject);
        }
  
        public void StartMoving()
        {
            _isMoving = true;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 1f);
        }
  
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

        public void StopMoving()
        {
            _isMoving = false;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 0f);
        }

        public void Look(Vector2 direction)
        {
            _lookRotation = GetRotation2DInDirection(direction);
        }

        public void LookAt(Vector3 position)
        {
            var direction = position - _transform.position;
            Look(direction);
        }

        public void Shoot()
        {
            BulletSpawner.TrySpawn();
        }

        public void Bomb()
        {
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
            head.rotation = _lookRotation;
        }

        private void FixedUpdate()
        {
            if (_isMoving)
                _rigidbody.velocity = _moveDirection;
        }
    }
}
