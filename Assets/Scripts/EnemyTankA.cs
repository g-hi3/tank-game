using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject head;
    private TankVision _tankVision;
    private FlipFlopRotator _rotator;
    private Transform _headTransform;

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
            var directionDelta = targetDirection - currentDirection;
            _headTransform.right = Vector3.MoveTowards(currentDirection, currentDirection + directionDelta, rotationSpeed);
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
