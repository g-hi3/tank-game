using System;
using JetBrains.Annotations;
using TankGame.Core.Bullet;
using UnityEngine;

namespace TankGame.Core.Bomb
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(Animator))]
    public class Bomb : MonoBehaviour, IDetonationTarget, IBulletTarget
    {
        private static readonly int TriggerNameExplosionTrigger = Animator.StringToHash("Explosion Trigger");
        [SerializeField] private float lifetimeSeconds;
        [SerializeField] private Vector2 _explosionScale;
        [NotNull] private Transform _transform = default!;
        [NotNull] private Animator _animator = default!;
        private float _remainingLifetimeSeconds;
        private bool _explosionActive;
        private bool _paused;

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
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
            _animator = GetComponent<Animator>()
                        ?? throw new InvalidOperationException($"Missing {nameof(Animator)} component!");
        }

        private void Start()
        {
            _remainingLifetimeSeconds = lifetimeSeconds;
        }

        private void Update()
        {
            if (_paused || _explosionActive)
                return;

            _remainingLifetimeSeconds -= Time.deltaTime;

            if (_remainingLifetimeSeconds <= 0f)
                Detonate();
        }

        [UsedImplicitly]
        private void OnPause()
        {
            _paused = true;
            _animator.speed = 0f;
        }

        [UsedImplicitly]
        private void OnResume()
        {
            _paused = false;
            _animator.speed = 1f;
        }

        private void OnTriggerStay2D(Collider2D other)
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