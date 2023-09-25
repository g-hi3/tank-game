using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.AI
{
    public class TankVision : MonoBehaviour
    {
        [SerializeField, Min(float.Epsilon)] private float radius;
        [SerializeField, Range(float.Epsilon, 360f)] private float angle;
        [SerializeField, Min(2)] private int fidelity;
        [SerializeField, Min(0)] private int maxReflectionCount;
        [SerializeField] private LayerMask reflectiveLayers;
        [SerializeField] private LayerMask collidingLayers;
        [SerializeField] private float castWidth;
        private Transform _transform;
        private CastInfo[] _casts = Array.Empty<CastInfo>();
        [NotNull] private readonly RaycastHit2D[] _collisionHits = new RaycastHit2D[5];
        [NotNull] private readonly RaycastHit2D[] _reflectionHits = new RaycastHit2D[5];

        public bool IsTargetVisible => _casts.Any(c => c.IsTargetHit);

        public Vector3? GetBestTargetDirection()
        {
            return _casts.FirstOrDefault(c => c.IsTargetHit)?.CastDirections[0];
        }

        private IEnumerable<CastInfo> GetCastsOrderedByTotalDistance()
        {
            var baseAngle = _transform.eulerAngles.z - 0.5f * angle;
            var origin = _transform.position;
            return Enumerable.Range(0, fidelity)
                .Select(i => {
                    var adjustedAngle = (baseAngle + angle * i / fidelity) * Mathf.Deg2Rad;
                    var x = Mathf.Cos(adjustedAngle);
                    var y = Mathf.Sin(adjustedAngle);
                    var direction = new Vector2(x, y);
                    return Multicast(origin, direction, radius, 0);
                })
                .Where(castInfo => castInfo != CastInfo.InvalidCast)
                .OrderBy(castInfo => castInfo.TotalDistance);
        }

        private CastInfo Multicast(Vector3 origin, Vector3 direction, float distance, int reflectCount)
        {
            var collidingHitCount = Physics2D.CircleCastNonAlloc(origin, castWidth, direction, _collisionHits, distance, collidingLayers);
            var collidingHit = collidingHitCount > 0 ? _collisionHits.First(rh => rh.distance > 0.1f) : default;
            var reflectingHitCount = Physics2D.CircleCastNonAlloc(origin, castWidth, direction, _reflectionHits, distance, reflectiveLayers);
            var reflectingHit = reflectingHitCount > 0 ? _reflectionHits.First(rh => rh.distance > 0.1f) : default;
            if (collidingHit != default
                && (reflectingHit == default
                    || collidingHit.distance < reflectingHit.distance))
            {
                var collidingCastInfo = CastInfo.CreateTargetHit(maxReflectionCount + 1);
                collidingCastInfo.CastDirections[reflectCount] = direction * collidingHit.distance;
                return collidingCastInfo;
            }

            if (reflectingHit != default)
            {
                var reflectedDirection = Vector2.Reflect(direction, reflectingHit.normal);
                var remainingDistance = distance - reflectingHit.distance;
                if (!(remainingDistance > 0f) || reflectCount == maxReflectionCount)
                {
                    var reflectingCastInfo = CastInfo.CreateNoHit(maxReflectionCount + 1);
                    reflectingCastInfo.CastDirections[reflectCount] = direction * reflectingHit.distance;
                    return reflectingCastInfo;
                }
                var remainingReflectCount = reflectCount + 1;
                var result = Multicast(reflectingHit.point + reflectingHit.normal * castWidth, reflectedDirection, remainingDistance, remainingReflectCount);
                if (result == CastInfo.InvalidCast)
                {
                    return CastInfo.InvalidCast;
                }

                result.CastDirections[reflectCount] = direction * reflectingHit.distance;
                return result;
            }

            var noHitCastInfo = CastInfo.CreateNoHit(maxReflectionCount + 1);
            noHitCastInfo.CastDirections[reflectCount] = direction * distance;
            return noHitCastInfo;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            _casts = GetCastsOrderedByTotalDistance().ToArray();
        }

        private void OnDrawGizmos()
        {
            if (_transform == null)
                return;
        
            var basePosition = _transform.position;
            foreach (var castInfo in _casts)
            {
                Gizmos.color = castInfo.IsTargetHit ? Color.yellow : Color.grey;
                var origin = Vector3.zero;
                foreach (var castDirection in castInfo.CastDirections)
                {
                    Gizmos.DrawRay(origin + basePosition, castDirection);
                    origin += castDirection;
                }
            }
        }
    }
}
