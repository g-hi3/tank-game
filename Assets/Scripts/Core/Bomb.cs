using UnityEngine;

namespace TankGame.Core
{
    public class Bomb : MonoBehaviour
    {
        private static readonly int TriggerNameExplosionTrigger = Animator.StringToHash("Explosion Trigger");
        [SerializeField] private float lifetimeSeconds;
        [SerializeField] private Vector2 _explosionScale;
        private Transform _transform;
        private Animator _animator;
        private float _remainingLifetimeSeconds;
        private bool _explosionActive;

        private void Explode()
        {
            _transform.localScale = _explosionScale;
            _animator.SetTrigger(TriggerNameExplosionTrigger);
            _explosionActive = true;
        }

        private void OnExplosionEnded()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _remainingLifetimeSeconds = lifetimeSeconds;
        }

        private void Update()
        {
            if (_explosionActive)
            {
                return;
            }
            _remainingLifetimeSeconds -= Time.deltaTime;
            if (_remainingLifetimeSeconds <= 0f)
            {
                Explode();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Bullet _))
            {
                Explode();
            }
            if (_explosionActive
                && other.TryGetComponent(out Bomb otherBomb))
            {
                otherBomb.Explode();
            }
            if (_explosionActive
                && other.TryGetComponent(out Tank tank))
            {
                tank.Die();
            }
        }

        public static Bomb FromBlueprint(BombBlueprint blueprint)
        {
            var bomb = Instantiate(blueprint.Prefab)
                .AddComponent<Bomb>()!;
            bomb._explosionScale = blueprint.ExplosionScale;
            return bomb;
        }
    }
}