using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Core
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectTemplate;
        [SerializeField] private LayerMask obstructionLayers;
        [SerializeField] private int capacity;
        private Transform _transform;
        private Collider2D _spawnArea;
        private readonly IList<GameObject> _activeObjects = new List<GameObject>();

        public bool TrySpawn(out GameObject spawned)
        {
            return TrySpawn(objectTemplate, out spawned);
        }

        public bool TrySpawn(GameObject template, out GameObject spawned)
        {
            if (IsCapacityReached() || IsSpawnAreaObstructed())
            {
                spawned = default;
                return false;
            }
    
            spawned = Spawn(template);
            return true;
        }

        private GameObject Spawn(GameObject template)
        {
            var spawned = Instantiate(template, _transform.position, _transform.rotation);
            _activeObjects.Add(spawned);
            return spawned;
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
            for (var index = _activeObjects.Count - 1; index >= 0; index--)
            {
                if (_activeObjects[index] == null)
                {
                    _activeObjects.RemoveAt(index);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_spawnArea == null)
            {
                return;
            }
            Gizmos.color = IsSpawnAreaObstructed() ? Color.red : Color.green;
            var spawnAreaBounds = _spawnArea.bounds;
            Gizmos.DrawWireCube(spawnAreaBounds.center, spawnAreaBounds.size);
        }
    }
}
