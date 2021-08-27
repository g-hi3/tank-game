using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
  
  [SerializeField] private GameObject objectTemplate;
  [SerializeField] private LayerMask obstructionLayers;
  [SerializeField] private int capacity;
  private Transform _transform;
  private Collider2D _spawnArea;
  private readonly IList<GameObject> _activeObjects = new List<GameObject>();

  public void Spawn()
  {
    if (IsCapacityReached()
        || IsSpawnAreaObstructed()) {
      return;
    }
    
    var spawnedObject = Instantiate(objectTemplate, _transform.position, _transform.rotation);
    _activeObjects.Add(spawnedObject);
  }
  
  private bool IsCapacityReached()
  {
    return _activeObjects.Count == capacity;
  }

  private bool IsSpawnAreaObstructed()
  {
    return _spawnArea.IsTouchingLayers(obstructionLayers.value);
  }

  private void Awake()
  {
    _transform = GetComponent<Transform>();
    _spawnArea = GetComponent<Collider2D>();
  }

  private void Update()
  {
    CleanObjectPool();
  }

  private void CleanObjectPool()
  {
    for (var index = _activeObjects.Count - 1; index >= 0; index--) {
      if (_activeObjects[index] == null) {
        _activeObjects.RemoveAt(index);
      }
    }
  }

  private void OnDrawGizmos()
  {
    if (_spawnArea == null) {
      return;
    }
    Gizmos.color = IsSpawnAreaObstructed() ? Color.red : Color.green;
    var spawnAreaBounds = _spawnArea.bounds;
    Gizmos.DrawWireCube(spawnAreaBounds.center, spawnAreaBounds.size);
  }
}
