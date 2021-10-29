using System.Collections;
using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject head;
    [SerializeField] private ObjectSpawner bulletSpawner;
    [SerializeField] private float heatUpTime;
    [SerializeField] private float coolDownTime;
    private TankVision _tankVision;
    private FlipFlopRotator _rotator;
    private Transform _headTransform;
    private Coroutine _shootingCoroutine;

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(heatUpTime);
        bulletSpawner.Spawn();
        yield return new WaitForSeconds(coolDownTime);
        _shootingCoroutine = null;
    }
    
    private void Awake()
    {
        _tankVision = GetComponentInChildren<TankVision>();
        _rotator = GetComponentInChildren<FlipFlopRotator>();
    }

    private void Start()
    {
        _headTransform = head.GetComponent<Transform>();
    }

    private void Update()
    {
        if (_tankVision.IsTargetVisible)
        {
            if (_rotator.enabled)
            {
                _rotator.enabled = false;
            }
            var currentDirection = _headTransform.right;
            var targetDirection = _tankVision.GetBestTargetDirection();

            if (Vector2.Distance(currentDirection.normalized, targetDirection.normalized) < 0.02f)
            {
                _shootingCoroutine ??= StartCoroutine(Shoot());
                return;
            }
            
            var expectedDirection = Vector3.MoveTowards(currentDirection, targetDirection, rotationSpeed);
            _headTransform.right = expectedDirection;
        }
        else
        {
            if (!_rotator.enabled)
            {
                _rotator.enabled = true;
            }
        }
    }
}
