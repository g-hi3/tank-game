using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Core.Bomb
{
    public class BombSpawner : MonoBehaviour
    {
        private readonly List<GameObject> _activeObjects = new();
        
        [field: SerializeField] public BombFactory Factory { get; private set; }
        [field: SerializeField] public uint Capacity { get; private set; }
        
        private bool CapacityReached => _activeObjects.Count >= Capacity;

        public bool TrySpawn()
        {
            if (CapacityReached)
                return false;

            var spawned = Factory.Make(transform);
            _activeObjects.Add(spawned);
            return true;
        }

        private void Update()
        {
            _activeObjects.RemoveAll(activeObject => activeObject == null);
        }
    }
}