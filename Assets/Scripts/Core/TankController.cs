using UnityEngine;

namespace TankGame.Core
{
    public class TankController : MonoBehaviour
    {
        private const float RotationZOffset = 270f;
        private static readonly int MoveSpeedAnimatorProperty = Animator.StringToHash("Move Speed");
        [SerializeField] private Transform head;
        [SerializeField] private float moveSpeed;
        [SerializeField] private ObjectSpawner bulletSpawner;
        [SerializeField] private ObjectSpawner bombSpawner;
        private Transform _transform;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private bool _isMoving;
        private Vector3 _moveDirection;
        private Quaternion _moveRotation;
        private Quaternion _lookRotation;

        private void OnExplosionEnded()
        {
            Destroy(gameObject);
        }
  
        private void StartMoving()
        {
            _isMoving = true;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 1f);
        }
  
        private void Move(Vector3 direction)
        {
            _moveDirection = moveSpeed * direction;
            _moveRotation = GetRotation2DInDirection(direction);
        }

        private static Quaternion GetRotation2DInDirection(Vector3 direction)
        {
            var rotationZ = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - RotationZOffset;
            return Quaternion.Euler(0f, 0f, rotationZ);
        }

        private void StopMoving()
        {
            _isMoving = false;
            _animator.SetFloat(MoveSpeedAnimatorProperty, 0f);
        }

        public void Look(Vector2 direction)
        {
            _lookRotation = GetRotation2DInDirection(direction);
        }

        private void LookAt(Vector3 position)
        {
            var direction = position - _transform.position;
            Look(direction);
        }

        private void Shoot()
        {
            bulletSpawner.Spawn();
        }

        private void Bomb()
        {
            bombSpawner.Spawn();
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
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
            {
                _rigidbody.velocity = _moveDirection;
            }
        }
    }
}
