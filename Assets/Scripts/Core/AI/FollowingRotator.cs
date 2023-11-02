using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.AI
{
    [RequireComponent(typeof(TankVision))]
    [RequireComponent(typeof(Transform))]
    public class FollowingRotator : TargetingRotator
    {
        //[NotNull] private TankVision _tankVision = default!;
        public MultithreadingTankVision _tankVision;
        [NotNull] private Transform _transform = default!;
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
            _transform.right = Vector3.MoveTowards(_transform.right, _targetDirection, RotationSpeed);
        }

        private void Awake()
        {
            //_tankVision = GetComponent<TankVision>()
            //              ?? throw new InvalidOperationException($"Missing {nameof(TankVision)} component!");
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_transform.position, _targetDirection);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_transform.position, _transform.rotation * Vector3.right);
        }
    }
}
