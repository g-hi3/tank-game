using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.AI
{
    [RequireComponent(typeof(Transform))]
    public class PathMover : Mover
    {
        [NotNull] private Transform _transform = default!;

        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public List<Transform> PathPoints { get; private set; }
        [field: SerializeField] public Transform CurrentTarget { get; private set; }

        /// <inheritdoc />
        public override void Move()
        {
            if (CurrentTarget == null)
                CurrentTarget = PathPoints?.FirstOrDefault();

            if (CurrentTarget == null)
                return;
            
            if (Vector2.Distance(transform.position, CurrentTarget.position) < 0.01f)
            {
                var nextTargetIndex = PathPoints.IndexOf(CurrentTarget) + 1;
                CurrentTarget = nextTargetIndex < PathPoints.Count
                    ? PathPoints[nextTargetIndex]
                    : PathPoints[0];
            }

            _transform.position = Vector2.MoveTowards(
                _transform.position,
                CurrentTarget.position,
                Speed * Time.deltaTime);
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>()
                         ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
        }
    }
}