using System.Collections.Generic;
using UnityEngine;

public class AITankInput : MonoBehaviour {

  [SerializeField] private List<Transform> _stops;
  [SerializeField] private float _movementSpeed;
  [SerializeField] private float _distanceDelta;
  private Transform _transform;
  private Transform _nextStop;
  private int _stopIndex;

  private Transform GetNextStop() {
    if (_stopIndex >= _stops.Count) {
      _stopIndex = 0;
    }
    return _stops[_stopIndex++];
  }

  private bool HasReachedStop() {
    var direction = _nextStop.position - _transform.position;
    return direction.magnitude <= _distanceDelta;
  }

  private void Awake() {
    _transform = GetComponent<Transform>();
  }

  private void Start() {
    _nextStop = GetNextStop();
  }

  private void FixedUpdate() {
    if (HasReachedStop()) {
      _nextStop = GetNextStop();
    }
    var direction = (_nextStop.position - _transform.position).normalized;
    _transform.Translate(Time.fixedDeltaTime * _movementSpeed * direction);
  }

}