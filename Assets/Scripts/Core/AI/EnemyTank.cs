using System.Collections;
using JetBrains.Annotations;
using TankGame.Core.Bullet;
using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Represents the AI of an enemy tank.
    /// </summary>
    public class EnemyTank : MonoBehaviour
    {
        [SerializeField] private float heatUpTime;
        [SerializeField] private float coolDownTime;
        private Coroutine _shootingCoroutine;
        private bool _paused;

        /// <summary>
        /// Component that controls the aim direction of the tank.
        /// </summary>
        [field: SerializeField] public TargetingRotator Rotator { get; private set; }

        /// <summary>
        /// Component that controls the movement of the tank.
        /// </summary>
        [field: SerializeField] public Mover Mover { get; private set; }

        /// <summary>
        /// Component that spawns bullets, when the AI decides to shoot.
        /// </summary>
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
