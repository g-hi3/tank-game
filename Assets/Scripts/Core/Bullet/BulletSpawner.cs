using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.Bullet
{
    /// <summary>
    /// This component handles the creation of bullets with a capacity.
    /// </summary>
    public class BulletSpawner : MonoBehaviour
    {
        [NotNull] private readonly List<GameObject> _activeObjects = new();

        /// <summary>
        /// This component creates the bullet game object.
        /// </summary>
        [field: SerializeField] public BulletFactory Factory { get; private set; }

        /// <summary>
        /// This component checks if the bullet can be spawned i.e. no objects will intersect with it at spawn.
        /// </summary>
        [field: SerializeField] public Collider2D SpawnArea { get; private set; }

        /// <summary>
        /// This layer mask filters object that can obstruct the spawn.
        /// </summary>
        [field: SerializeField] public LayerMask ObstructionLayers { get; private set; }

        /// <summary>
        /// Represents the total number of bullets that can be spawned by this component.
        /// </summary>
        [field: SerializeField] public uint Capacity { get; private set; }

        private bool CapacityReached => _activeObjects.Count >= Capacity;
        private bool Obstructed => SpawnArea!.IsTouchingLayers(ObstructionLayers);

        /// <summary>
        /// Attempts to spawn a bullet.
        /// This method won't spawn a bullet, if the spawn area is obstructed or the capacity has been reached.
        /// </summary>
        /// <returns><c>true</c> when a bullet has been spawned; otherwise <c>false</c></returns>
        public bool TrySpawn()
        {
            if (Obstructed || CapacityReached)
                return false;

            var spawned = Factory!.Make(SpawnArea!.transform);
            _activeObjects.Add(spawned);
            return true;
        }

        private void Awake()
        {
            Factory ??= GetComponent<BulletFactory>()
                        ?? throw new InvalidOperationException($"Missing {nameof(BulletFactory)} component!");
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