using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Core.Bullet
{
    public class BulletSpawner : MonoBehaviour
    {
        private readonly List<GameObject> _activeObjects = new();
        
        [field: SerializeField] public BulletFactory Factory { get; private set; }
        [field: SerializeField] public Collider2D SpawnArea { get; private set; }
        [field: SerializeField] public LayerMask ObstructionLayers { get; private set; }
        [field: SerializeField] public uint Capacity { get; private set; }

        private bool CapacityReached => _activeObjects.Count >= Capacity;
        private bool Obstructed => SpawnArea.IsTouchingLayers(ObstructionLayers);

        public bool TrySpawn()
        {
            if (Obstructed || CapacityReached)
                return false;

            var spawned = Factory.Make(SpawnArea.transform);
            _activeObjects.Add(spawned);
            return true;
        }

        private void Update()
        {
            _activeObjects.RemoveAll(activeObject => activeObject == null);
        }

        private void OnDrawGizmos()
        {
            if (SpawnArea == null)
                return;
            
            Gizmos.color = Obstructed ? Color.red : Color.green;
            var spawnAreaBounds = SpawnArea.bounds;
            Gizmos.DrawWireCube(spawnAreaBounds.center, spawnAreaBounds.size);
        }
    }
}