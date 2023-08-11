using UnityEngine;

namespace TankGame.Core
{
    public class Bullet : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private uint ricochetCount;
        private Transform _transform;
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
