using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private Transform _transform;
    private TankVision _tankVision;
    private FlipFlopRotator _rotator;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _tankVision = GetComponentInChildren<TankVision>();
        _rotator = GetComponentInChildren<FlipFlopRotator>();
    }

    private void Update()
    {
        if (_tankVision.IsTargetVisible)
        {
            if (_rotator.enabled)
            {
                _rotator.enabled = false;
            }
            var currentDirection = _transform.right;
            var targetDirection = _tankVision.GetBestTargetDirection();
            var directionDelta = targetDirection - currentDirection;
            _transform.right = Vector3.MoveTowards(currentDirection, currentDirection + directionDelta, rotationSpeed);
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
