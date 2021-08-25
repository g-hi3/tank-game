using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
  
  [SerializeField] private GameObject bulletTemplate;
  [SerializeField] private LayerMask obstructionLayers;
  [SerializeField] private int capacity;
  private Transform _transform;
  private Collider2D _spawnArea;
  private readonly IList<GameObject> _activeBullets = new List<GameObject>();

  public void Spawn()
  {
    if (IsCapacityReached()
        || IsSpawnAreaObstructed()) {
      return;
    }
    
    var bullet = Instantiate(bulletTemplate, _transform.position, _transform.rotation);
    _activeBullets.Add(bullet);
  }
  
  private bool IsCapacityReached()
  {
    return _activeBullets.Count == capacity;
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
    CleanBulletPool();
  }

  private void CleanBulletPool()
  {
    for (var index = _activeBullets.Count - 1; index >= 0; index--) {
      if (_activeBullets[index] == null) {
        _activeBullets.RemoveAt(index);
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
