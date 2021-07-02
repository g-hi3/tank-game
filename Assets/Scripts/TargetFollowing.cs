using System.Linq;
using UnityEngine;

public class TargetFollowing : MonoBehaviour {

  [SerializeField] private LayerMask _reflectingObjects;
  [SerializeField] private LayerMask _targetObjects;
  [SerializeField] private int _rayCount;
  [SerializeField] private int _ricochetCount;
  [SerializeField] private float _castWidth;
  private Transform _transform;

  private void Awake() {
    _transform = GetComponent<Transform>();
  }

  private void Update() {
    var leastRicochets = _ricochetCount + 1;
    var direction = Vector3.zero;
    for (var i = -_rayCount / 2; i < _rayCount / 2; i++) {
      var currentDirection = GetRelativeDirection(new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0f));
      var ricochetCount = CastRay(_transform.position, currentDirection, 0);
      if (ricochetCount != -1
        && ricochetCount < leastRicochets) {
        leastRicochets = ricochetCount;
        direction = currentDirection;
      }
    }
    SendMessage("Look", (Vector2)direction);
  }

  private Vector3 GetRelativeDirection(Vector3 originalDirection) {
    return _transform.TransformDirection(originalDirection);
  }

  private int CastRay(Vector3 currentPosition, Vector3 rayDirection, int ricochetCount) {
    if (ricochetCount >= _ricochetCount) {
      return -1;
    }
    var targetHit = CircleCast2DForPlayer(currentPosition, rayDirection);
    if (targetHit != default) {
      DebugDrawRay(currentPosition, rayDirection, targetHit, Color.red);
      return ricochetCount;
    }
    var raycastHit = CircleCast2DForWalls(currentPosition, rayDirection);
    if (raycastHit == default) {
      return -1;
    }
    var ricochetDirection = Vector2.Reflect(rayDirection, raycastHit.normal);
    var ricochetCountUntilHit = CastRay(raycastHit.point, ricochetDirection, ricochetCount + 1);
    if (ricochetCountUntilHit > 0) {
      DebugDrawRay(currentPosition, rayDirection, raycastHit, GetRaycastColor(ricochetCountUntilHit));
    }
    return ricochetCountUntilHit;
  }

  private RaycastHit2D CircleCast2DForPlayer(Vector3 currentPosition, Vector3 rayDirection) {
    var playerHit = Physics2D.CircleCast(currentPosition, _castWidth, rayDirection, Mathf.Infinity, _targetObjects);
    if (playerHit == default) {
      return default;
    }
    var firstWallHit = CircleCast2DForWalls(currentPosition, rayDirection);
    return playerHit.distance < firstWallHit.distance
      ? playerHit
      : default;
  }

  private void DebugDrawRay(Vector3 currentPosition, Vector3 rayDirection, RaycastHit2D raycastHit, Color color) {
    var targetPosition = (0.5f * _castWidth + raycastHit.distance) * rayDirection.normalized;
    Debug.DrawRay(currentPosition, targetPosition, color);
  }

  private RaycastHit2D CircleCast2DForWalls(Vector3 currentPosition, Vector3 rayDirection) {
    var allRaycastHits = Physics2D.CircleCastAll(currentPosition, _castWidth, rayDirection, Mathf.Infinity, _reflectingObjects);
    return allRaycastHits.FirstOrDefault(h => h.distance > float.Epsilon);
  }

  private static Color GetRaycastColor(int ricochetNumber) {
    switch (ricochetNumber) {
      case 0: return Color.blue;
      case 1: return Color.cyan;
      case 2: return Color.green;
      case 3: return Color.yellow;
      default: return Color.black;
    }
  }

}
