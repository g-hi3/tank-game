using System.Collections;
using TankGame.Core;
using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private ObjectSpawner bulletSpawner;
    [SerializeField] private float heatUpTime;
    [SerializeField] private float coolDownTime;
    private Coroutine _shootingCoroutine;
    
    [field: SerializeField] public TargetingRotator Rotator { get; private set; }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(heatUpTime);
        bulletSpawner.Spawn();
        yield return new WaitForSeconds(coolDownTime);
        _shootingCoroutine = null;
    }

    private void Awake()
    {
        Rotator = GetComponentInChildren<TargetingRotator>();
    }

    private void Update()
    {
        if (Rotator.IsTargetInSight())
            _shootingCoroutine ??= StartCoroutine(Shoot());
        else
            Rotator.Rotate();
    }
}
