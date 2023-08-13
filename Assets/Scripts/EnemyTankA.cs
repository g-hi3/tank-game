using System.Collections;
using TankGame.Core;
using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject head;
    [SerializeField] private ObjectSpawner bulletSpawner;
    [SerializeField] private float heatUpTime;
    [SerializeField] private float coolDownTime;
    private TankVision _tankVision;
    private Rotator _rotator;
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
        _rotator = GetComponentInChildren<Rotator>();
    }

    private void Start()
    {
        _headTransform = head.GetComponent<Transform>();
    }

    private void Update()
    {
        if (_tankVision.IsTargetVisible)
        {
            Debug.Log("Target is visible(?)");
            Vector2 currentDirection = _headTransform.right;
            Vector2 targetDirection = _tankVision.GetBestTargetDirection().normalized;
            
            if (Vector2.Distance(currentDirection, targetDirection) < 0.02f)
            {
                _shootingCoroutine ??= StartCoroutine(Shoot());
                return;
            }
 
            _headTransform.right = Vector2.MoveTowards(currentDirection, targetDirection, rotationSpeed);
        }
        else
        {
            Debug.Log("Using fallback rotator.");
            _rotator.Rotate();
        }
    }
}
