﻿using System.Linq;
using UnityEngine;

public class TargetFollowing : MonoBehaviour {

  [SerializeField] private LayerMask _reflectingObjects;
  [SerializeField] private LayerMask _targetObjects;
  [SerializeField] private int _numberOfRays;
  [SerializeField] private int _numberOfRicochets;
  [SerializeField] private float _castWidth;
  private Transform _transform;

  private void Awake() {
    _transform = GetComponent<Transform>();
  }

  private void Update() {
    for (var i = -_numberOfRays / 2; i < _numberOfRays / 2; i++) {
      var direction = GetRelativeDirection(new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0f));
      CastRay(_transform.position, direction, 0);
    }
  }

  private Vector3 GetRelativeDirection(Vector3 originalDirection) {
    return _transform.TransformDirection(originalDirection);
  }

  private bool CastRay(Vector3 currentPosition, Vector3 rayDirection, int numberOfRicochets) {
    if (numberOfRicochets >= _numberOfRicochets) {
      return false;
    }
    var targetHit = CircleCast2DOrDefaultIfImmediateCollision(currentPosition, rayDirection, _targetObjects.value);
    if (targetHit != default) {
      var targetPosition = rayDirection * targetHit.distance;
      Debug.DrawRay(currentPosition, targetPosition, Color.red);
      return true;
    }
    var raycastHit = CircleCast2DOrDefaultIfImmediateCollision(currentPosition, rayDirection, _reflectingObjects.value);
    if (raycastHit == default) {
      return false;
    }
    var ricochetDirection = Vector2.Reflect(rayDirection, raycastHit.normal);
    var hitTarget = CastRay(raycastHit.point, ricochetDirection, numberOfRicochets + 1);
    if (hitTarget) {
      var targetPosition = rayDirection * raycastHit.distance;
      Debug.DrawRay(currentPosition, targetPosition, GetRaycastColor(numberOfRicochets));
    }
    return hitTarget;
  }

  private RaycastHit2D CircleCast2DOrDefaultIfImmediateCollision(Vector3 currentPosition, Vector3 rayDirection, LayerMask targetLayer) {
    var allRaycastHits = Physics2D.CircleCastAll(currentPosition, _castWidth, rayDirection, Mathf.Infinity, targetLayer);
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
