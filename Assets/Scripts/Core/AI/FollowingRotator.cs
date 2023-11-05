using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Tries to find a player and aims at them.
    /// </summary>
    [RequireComponent(typeof(TankVision))]
    [RequireComponent(typeof(Transform))]
    public class FollowingRotator : TargetingRotator
    {
        [NotNull] private TankVision _tankVision = default!;
        [NotNull] private Transform _transform = default!;
        private Vector2 _targetDirection;

        /// <summary>
        /// Determines the speed in which the tank aim will rotate towards the player.
        /// </summary>
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
                   && Vector2.Distance(_transform.right, targetDirection.Value.normalized) < 0.02f;
        }

        /// <summary>
        /// When a player is in sight, this method will rotate the tank's aim towards the player; otherwise,
        /// <see cref="FallbackRotator"/> will be used.
        /// </summary>
        public override void Rotate()
        {
            if (_tankVision.IsTargetVisible)
                RotateTowardsTarget();
            else if (FallbackRotator)
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
            _tankVision = GetComponent<TankVision>()
                          ?? throw new InvalidOperationException($"Missing {nameof(TankVision)} component!");
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
        }

        private void OnDrawGizmos()
        {
            var transformPosition = _transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transformPosition, _targetDirection);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transformPosition, _transform.rotation * Vector3.right);
        }
    }
}
