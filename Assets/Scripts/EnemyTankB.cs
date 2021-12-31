using System.Collections.Generic;
using UnityEngine;

public class EnemyTankB : MonoBehaviour
{
    [SerializeField] private List<Transform> path;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float distanceEpsilon;
    private Transform _transform;
    private Transform _currentNode;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (path.Count == 0)
        {
            return;
        }
        
        if (_currentNode == null)
        {
            _currentNode = path[0];
        }

        if (Vector2.Distance(_transform.position, _currentNode.position) < distanceEpsilon)
        {
            var currentPathIndex = path.IndexOf(_currentNode) + 1;
            _currentNode = currentPathIndex < path.Count ? path[currentPathIndex] : path[0];
        }

        _transform.position = Vector3.MoveTowards(_transform.position, _currentNode.position, movementSpeed * Time.deltaTime);
    }
}
