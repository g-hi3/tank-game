using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.Bomb
{
    /// <summary>
    /// This component handles <see cref="Bomb"/> spawning with capacity.
    /// The component will track the spawned game objects and refuse to spawn more, if the capacity has been reached.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class BombSpawner : MonoBehaviour
    {
        [NotNull] private readonly List<GameObject> _activeObjects = new();
        [NotNull] private Transform _transform = default!;

        /// <summary>
        /// This component will create bomb game objects.
        /// </summary>
        /// <remarks>
        /// This property can be set from the editor. If at <see cref="Awake"/>, no value has been set, the component
        /// will attempt to find a factory on the current game object.
        /// </remarks>
        [field: SerializeField] public BombFactory Factory { get; private set; }

        /// <summary>
        /// Represents the capacity for spawning bombs.
        /// </summary>
        [field: SerializeField] public uint Capacity { get; private set; }

        private bool CapacityReached => _activeObjects.Count >= Capacity;

        /// <summary>
        /// Attempts to spawn a new bomb.
        /// A bomb can't be spawned if the capacity has been reached.
        /// </summary>
        /// <remarks>
        /// The bomb created will use this component's position and rotation.
        /// </remarks>
        /// <returns><c>true</c> if a bomb was spawned; otherwise <c>false</c></returns>
        public bool TrySpawn()
        {
            if (CapacityReached)
                return false;

            var spawned = Factory!.Make(_transform);
            _activeObjects.Add(spawned);
            return true;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>()
                ?? throw new InvalidOperationException($"Missing {nameof(Transform)} component!");
            Factory ??= GetComponent<BombFactory>()
                ?? throw new InvalidOperationException($"Missing {nameof(BombFactory)} component!");
        }

        private void Update()
        {
            _activeObjects.RemoveAll(activeObject => activeObject == null);
        }
    }
}