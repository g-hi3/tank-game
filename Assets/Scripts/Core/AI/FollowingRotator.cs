using UnityEngine;

namespace TankGame.Core.AI
{
    public class FollowingRotator : TargetingRotator
    {
        private TankVision _tankVision;
        private Vector2 _targetDirection;
        
        [field: SerializeField] public float RotationSpeed { get; private set; }

        /// <summary>
        /// Rotator to be used when no target is in sight
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Rotator to be used when no target is in sight")]
        public Rotator FallbackRotator { get; private set; }

        /// <inheritdoc />
        public override bool IsTargetInSight()
        {
            var targetDirection = _tankVision.GetBestTargetDirection();
            return targetDirection.HasValue
                   && Vector2.Distance(transform.right, targetDirection.Value.normalized) < 0.02f;
        }

        public override void Rotate()
        {
            if (_tankVision.IsTargetVisible)
                RotateTowardsTarget();
            else
                FallbackRotator.Rotate();
        }

        private void RotateTowardsTarget()
        {
            var bestTargetDirection = _tankVision.GetBestTargetDirection();
            if (bestTargetDirection == null)
                return;

            _targetDirection = bestTargetDirection.Value.normalized;
            transform.right = Vector3.MoveTowards(transform.right, _targetDirection, RotationSpeed);
        }

        private void Awake()
        {
            _tankVision = GetComponent<TankVision>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, _targetDirection);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, transform.rotation * Vector3.right);
        }
    }
}
