using UnityEngine;

namespace TankGame.Core
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private uint ricochetCount;
        private Transform _transform;
        private uint _remainingRicochetCount;
        private Vector2 _velocity;

        private void ReflectFrom(ContactPoint2D contactPoint)
        {
            var reflected = Vector2.Reflect(_transform.right, contactPoint.normal);
            var rotationZ = 90f - Mathf.Atan2(reflected.x, reflected.y) * Mathf.Rad2Deg;
            _transform.eulerAngles = new Vector3(0f, 0f, rotationZ);
            _remainingRicochetCount--;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            _remainingRicochetCount = ricochetCount;
        }

        private void Update()
        {
            if (_remainingRicochetCount < 1)
            {
                Destroy(gameObject);
            }
            _transform.Translate(Time.deltaTime * moveSpeed * Vector3.right);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Bullet _))
            {
                Destroy(gameObject);
                return;
            }

            if (other.gameObject.TryGetComponent(out Tank tank))
            {
                tank.Die();
                Destroy(gameObject);
                return;
            }
    
            if (gameObject.layer == other.gameObject.layer)
            {
                Physics2D.IgnoreCollision(other.collider, other.otherCollider);
                return;
            }

            ReflectFrom(other.contacts[0]);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bomb _))
                Destroy(gameObject);
        }

        public static Bullet FromBlueprint(BulletBlueprint blueprint, Vector2 bulletRotation)
        {
            var bullet = Instantiate(blueprint.Prefab)
                .AddComponent<Bullet>()!;
            bullet._remainingRicochetCount = blueprint.RicochetCount;
            bullet._velocity = bulletRotation.normalized * blueprint.Speed;
            return bullet;
        }
    }
}
