using System.Collections;
using JetBrains.Annotations;
using TankGame.Core.Bullet;
using UnityEngine;

namespace TankGame.Core.AI
{
    public class EnemyTank : MonoBehaviour
    {
        [SerializeField] private float heatUpTime;
        [SerializeField] private float coolDownTime;
        private Coroutine _shootingCoroutine;
        private bool _paused;

        [field: SerializeField] public TargetingRotator Rotator { get; private set; }
        [field: SerializeField] public Mover Mover { get; private set; }
        [field: SerializeField] public BulletSpawner BulletSpawner { get; private set; }

        private IEnumerator Shoot()
        {
            yield return new WaitForSeconds(heatUpTime);

            while (_paused)
                yield return null;

            if (BulletSpawner.TrySpawn())
                yield return new WaitForSeconds(coolDownTime);

            _shootingCoroutine = null;
        }

        private void Awake()
        {
            Rotator = GetComponentInChildren<TargetingRotator>();
            Mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (_paused)
                return;

            if (Rotator.IsTargetInSight())
                _shootingCoroutine ??= StartCoroutine(Shoot());
            else
                Rotator.Rotate();

            if (Mover != null)
                Mover.Move();
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
    }
}
