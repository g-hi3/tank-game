using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private IEnumerable<CastInfo> _casts = Enumerable.Empty<CastInfo>();

    public bool IsTargetVisible => _casts.Any(c => c.IsTargetHit);

    public Vector3 GetBestTargetDirection()
    {
        var optimalCast = _casts.FirstOrDefault(c => c.IsTargetHit);
        return optimalCast != default
            ? optimalCast.CastDirections[0]
            : _transform.localEulerAngles;
    }

    private IEnumerable<CastInfo> GetCastsOrderedByTotalDistance()
    {
        var baseAngle = -_transform.localEulerAngles.z - 0.5f * angle;
        var origin = _transform.position;
        return Enumerable.Range(0, fidelity)
            .Select(i => {
                var adjustedAngle = (baseAngle + angle * i / fidelity) * Mathf.Deg2Rad;
                var x = -Mathf.Cos(adjustedAngle);
                var y = Mathf.Sin(adjustedAngle);
                var direction = new Vector2(x, y);
                return Multicast(origin, direction, radius, 0);
            })
            .Where(castInfo => castInfo != CastInfo.InvalidCast)
            .OrderBy(castInfo => castInfo.TotalDistance);
    }

    private CastInfo Multicast(Vector3 origin, Vector3 direction, float distance, int reflectCount)
    {
        var collidingHit = Physics2D.CircleCastAll(origin, castWidth, direction, distance, collidingLayers)
            .FirstOrDefault(rh => rh.distance > 0.1f);
        var reflectingHit = Physics2D.CircleCastAll(origin, castWidth, direction, distance, reflectiveLayers)
            .FirstOrDefault(rh => rh.distance > 0.1f);
        if (collidingHit != default
            && (reflectingHit == default
                || collidingHit.distance < reflectingHit.distance))
        {
            var collidingCastInfo = new CastInfo {
                CastDirections = new Vector3[maxReflectionCount + 1],
                IsTargetHit = true
            };
            collidingCastInfo.CastDirections[reflectCount] = direction * collidingHit.distance;
            return collidingCastInfo;
        }

        if (reflectingHit != default)
        {
            var reflectedDirection = Vector2.Reflect(direction, reflectingHit.normal);
            var remainingDistance = distance - reflectingHit.distance;
            if (!(remainingDistance > 0f) || reflectCount == maxReflectionCount)
            {
                var reflectingCastInfo = new CastInfo {
                    CastDirections = new Vector3[maxReflectionCount + 1]
                };
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

        var noHitCastInfo = new CastInfo {
            CastDirections = new Vector3[maxReflectionCount + 1]
        };
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
        {
            return;
        }

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
