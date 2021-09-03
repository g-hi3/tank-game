using System.Linq;
using UnityEngine;

public class TargetFollowing : MonoBehaviour
{
    [SerializeField] private LayerMask reflectingObjects;
    [SerializeField] private LayerMask targetObjects;
    [SerializeField] private string targetTag;
    [SerializeField] private int rayCount;
    [SerializeField] private int ricochetCount;
    [SerializeField] private float castWidth;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        var leastRicochets = ricochetCount + 1;
        var direction = Vector3.zero;
        for (var i = -rayCount / 2; i < rayCount / 2; i++)
        {
            var currentDirection = GetRelativeDirection(new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0f));
            var rayRicochetCount = CastRay(_transform.position, currentDirection, 0);
            if (rayRicochetCount == -1
                || rayRicochetCount >= leastRicochets)
            {
                continue;
            }
            leastRicochets = rayRicochetCount;
            direction = currentDirection;
        }
        SendMessage("Look", (Vector2)direction);
    }

    private Vector3 GetRelativeDirection(Vector3 originalDirection)
    {
        return _transform.TransformDirection(originalDirection);
    }

    private int CastRay(Vector3 currentPosition, Vector3 rayDirection, int rayRicochetCount)
    {
        if (rayRicochetCount >= ricochetCount)
        {
            return -1;
        }
        var targetHit = CircleCast2DForPlayer(currentPosition, rayDirection);
        if (targetHit != default)
        {
            DebugDrawRay(currentPosition, rayDirection, targetHit, Color.red);
            return rayRicochetCount;
        }
        var raycastHit = CircleCast2DForWalls(currentPosition, rayDirection);
        if (raycastHit == default)
        {
            return -1;
        }
        var ricochetDirection = Vector2.Reflect(rayDirection, raycastHit.normal);
        var ricochetCountUntilHit = CastRay(
            raycastHit.point,
            ricochetDirection,
            rayRicochetCount + 1);
        if (ricochetCountUntilHit > 0)
        {
            DebugDrawRay(currentPosition, rayDirection, raycastHit, GetRaycastColor(ricochetCountUntilHit));
        }
        return ricochetCountUntilHit;
    }

    private RaycastHit2D CircleCast2DForPlayer(Vector3 currentPosition, Vector3 rayDirection)
    {
        var playerHit = Physics2D.CircleCastAll(
                currentPosition,
                castWidth,
                rayDirection,
                Mathf.Infinity,
                targetObjects)
            .FirstOrDefault(hit => hit.transform.CompareTag(targetTag));
        if (playerHit == default)
        {
            return default;
        }
        var firstWallHit = CircleCast2DForWalls(currentPosition, rayDirection);
        return playerHit.distance < firstWallHit.distance
            ? playerHit
            : default;
    }

    private void DebugDrawRay(Vector3 currentPosition, Vector3 rayDirection, RaycastHit2D raycastHit, Color color)
    {
        var targetPosition = (0.5f * castWidth + raycastHit.distance) * rayDirection.normalized;
        Debug.DrawRay(currentPosition, targetPosition, color);
    }

    private RaycastHit2D CircleCast2DForWalls(Vector3 currentPosition, Vector3 rayDirection)
    {
        var allRaycastHits = Physics2D.CircleCastAll(
            currentPosition,
            castWidth,
            rayDirection,
            Mathf.Infinity,
            reflectingObjects);
        return allRaycastHits.FirstOrDefault(h => h.distance > float.Epsilon);
    }

    private static Color GetRaycastColor(int ricochetNumber)
    {
        return ricochetNumber switch
        {
            0 => Color.blue,
            1 => Color.cyan,
            2 => Color.green,
            3 => Color.yellow,
            _ => Color.black
        };
    }
}
