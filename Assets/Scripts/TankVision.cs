using System.Linq;
using UnityEngine;

public class TankVision : MonoBehaviour
{
    [SerializeField, Min(float.Epsilon)] private float radius;
    [SerializeField, Range(float.Epsilon, 360f)] private float angle;
    [SerializeField, Min(2)] private int fidelity;
    [SerializeField, Min(0)] private int reflectionCount;
    [SerializeField] private LayerMask reflectiveLayers;
    [SerializeField] private LayerMask collidingLayers;
    [SerializeField] private float castWidth;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void OnDrawGizmos()
    {
        if (_transform == null)
        {
            return;
        }
        var baseAngle = _transform.rotation.z - 0.5f * angle;
        var origin = _transform.position;
        var bestCastInfo = Enumerable.Range(0, fidelity)
            .Select(i => {
                var adjustedAngle = (baseAngle + angle * i / fidelity) * Mathf.Deg2Rad;
                var x = -Mathf.Cos(adjustedAngle);
                var y = Mathf.Sin(adjustedAngle);
                var direction = new Vector2(x, y);
                return new {
                    TotalDistance = Multicast(origin, direction),
                    Direction = direction
                };
            })
            .Where(castInfo => castInfo.TotalDistance > 0f)
            .OrderBy(castInfo => castInfo.TotalDistance)
            .FirstOrDefault();
        if (bestCastInfo == default)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_transform.position, bestCastInfo.Direction);
    }

    private float Multicast(Vector3 origin, Vector3 direction)
    {
        var collidingHit = Physics2D.CircleCast(origin, castWidth, direction, radius, collidingLayers);
        var reflectingHit = Physics2D.CircleCast(origin, castWidth, direction, radius, reflectiveLayers);
        if (collidingHit != default
            && (reflectingHit == default
                || collidingHit.distance < reflectingHit.distance))
        {
            DrawRayWithColor(origin, direction * collidingHit.distance, Color.yellow);
            return collidingHit.distance;
        }

        if (reflectingHit != default)
        {
            var reflectedDirection = Vector2.Reflect(direction, reflectingHit.normal);
            var remainingDistance = radius - reflectingHit.distance;
            if (!(remainingDistance > 0f) || reflectionCount <= 0)
            {
                DrawRayWithColor(origin, direction * reflectingHit.distance, Color.grey);
                return 0f;
            }
            var remainingReflectCount = reflectionCount - 1;
            var result = MulticastRecursive(reflectingHit.point + reflectingHit.normal * castWidth, reflectedDirection, remainingDistance, remainingReflectCount);
            DrawRayWithColor(origin, direction * reflectingHit.distance, result > 0f ? Color.yellow : Color.grey);
            return result > 0f ? result + reflectingHit.distance : 0f;
        }

        DrawRayWithColor(origin, direction * radius, Color.grey);
        return 0f;
    }

    private float MulticastRecursive(Vector3 origin, Vector3 direction, float distance, int reflectCount)
    {
        var collidingHit = Physics2D.CircleCastAll(origin, castWidth, direction, distance, collidingLayers)
            .FirstOrDefault(rh => rh.distance > 0.1f);
        var reflectingHit = Physics2D.CircleCastAll(origin, castWidth, direction, distance, reflectiveLayers)
            .FirstOrDefault(rh => rh.distance > 0.1f);
        if (collidingHit != default
            && (reflectingHit == default
                || collidingHit.distance < reflectingHit.distance))
        {
            DrawRayWithColor(origin, direction * collidingHit.distance, Color.yellow);
            return collidingHit.distance;
        }

        if (reflectingHit != default)
        {
            var reflectedDirection = Vector2.Reflect(direction, reflectingHit.normal);
            var remainingDistance = distance - reflectingHit.distance;
            if (!(remainingDistance > 0f) || reflectCount <= 0)
            {
                DrawRayWithColor(origin, direction * reflectingHit.distance, Color.grey);
                return 0f;
            }
            var remainingReflectCount = reflectCount - 1;
            var result = MulticastRecursive(reflectingHit.point, reflectedDirection, remainingDistance, remainingReflectCount);
            DrawRayWithColor(origin, direction * reflectingHit.distance, result > 0f ? Color.yellow : Color.grey);
            return result > 0f ? result + reflectingHit.distance : 0f;
        }

        DrawRayWithColor(origin, direction * distance, Color.grey);
        return 0f;
    }

    private static void DrawRayWithColor(Vector3 origin, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(origin, direction);
    }
}
