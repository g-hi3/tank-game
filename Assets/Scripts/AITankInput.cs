using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AITankInput : MonoBehaviour {

    [SerializeField] private List<Transform> stops;
    [SerializeField] private float distanceDelta;
    private Transform _transform;
    private Transform _nextStop;
    private int _stopIndex;
    private Vector3 _movement;

    private Transform GetNextStop()
    {
        if (_stopIndex >= stops.Count)
        {
            _stopIndex = 0;
        }
        return stops[_stopIndex++];
    }

    private bool HasReachedStop()
    {
        var direction = _nextStop.position - _transform.position;
        return direction.magnitude <= distanceDelta;
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _nextStop = GetNextStop();
        SendMessage("StartMoving");
    }

    private void Update() {
        if (HasReachedStop())
        {
            _nextStop = GetNextStop();
        }

        _movement = (_nextStop.position - _transform.position).normalized;
        SendMessage("Move", _movement);
    }

}
