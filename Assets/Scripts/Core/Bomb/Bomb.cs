using UnityEngine;

namespace TankGame.Core.Bomb
{
    public class Bomb : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        private static readonly int TriggerNameExplosionTrigger = Animator.StringToHash("Explosion Trigger");
        [SerializeField] private float lifetimeSeconds;
        [SerializeField] private Vector2 _explosionScale;
        private Transform _transform;
        private Animator _animator;
        private float _remainingLifetimeSeconds;
        private bool _explosionActive;

        public void OnDetonationHit() => Detonate();

        public void OnBulletHit() => Detonate();

        private void Detonate()
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
                Detonate();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_explosionActive && other.TryGetComponent(out IDetonationTarget detonationTarget))
                detonationTarget.OnDetonationHit();
        }

        public static Bomb FromBlueprint(BombBlueprint blueprint, GameObject bombObject)
        {
            var bomb = GetOrAddBomb(bombObject);
            bomb._explosionScale = blueprint.ExplosionScale;
            return bomb;
        }

        private static Bomb GetOrAddBomb(GameObject gameObject)
        {
            return gameObject.TryGetComponent<Bomb>(out var component)
                ? component
                : gameObject.AddComponent<Bomb>();
        }
    }
}