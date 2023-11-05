using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Moves the enemy tank along a path of <see cref="Transform"/>s.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class PathMover : Mover
    {
        [NotNull] private Transform _transform = default!;

        /// <summary>
        /// Controls the speed at which the tank is moved.
        /// </summary>
        [field: SerializeField] public float Speed { get; private set; }

        /// <summary>
        /// Ordered list of points that the tank should follow.
        /// </summary>
        /// <remarks>
        /// This property contains <see cref="Transform"/>s instead of vectors, because it makes it easier to edit in
        /// the editor.
        /// </remarks>
        [field: SerializeField] public List<Transform> PathPoints { get; private set; }

        /// <summary>
        /// The target that the tank will move to when <see cref="Move"/> is called.
        /// </summary>
        /// <remarks>
        /// The value will be changed by <see cref="Move"/>, but is also settable via the editor.
        /// </remarks>
        [field: SerializeField] public Transform CurrentTarget { get; private set; }

        /// <summary>
        /// Moves to the position of <see cref="CurrentTarget"/>.
        /// When the target position has been reached, the next position in <see cref="PathPoints"/> will be used. After
        /// the last point, the tank will move back to the first point and continue in a loop.
        /// </summary>
        /// <remarks>
        /// The tank won't move if <see cref="CurrentTarget"/> isn't set and <see cref="PathPoints"/> is empty.
        /// Additionally, a point is considered reached when it's in close proximity of the tank.
        /// </remarks>
        public override void Move()
        {
            if (CurrentTarget == null)
                CurrentTarget = PathPoints?.FirstOrDefault();

            if (CurrentTarget == null)
                return;
            
            if (Vector2.Distance(_transform.position, CurrentTarget.position) < 0.01f)
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