using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankGame.Core.Spawn
{
    /// <summary>
    /// Spawns player objects.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        /// <summary>
        /// Component that controls the instantiation of new player objects based on input.
        /// </summary>
        [field: SerializeField] public PlayerInputManager PlayerInputManager { get; private set; }

        /// <summary>
        /// Event that fires when a new player spawned.
        /// </summary>
        [field: SerializeField] public UnityEvent<GameObject> PlayerSpawned { get; private set; }

        private void Awake()
        {
            PlayerInputManager = GetComponent<PlayerInputManager>()
                ?? throw new InvalidOperationException($"Missing {nameof(PlayerInputManager)} component!");
        }

        private void Start()
        {
            PlayerInputManager!.onPlayerJoined += SpawnPlayer;
        }

        private void SpawnPlayer([NotNull] PlayerInput spawningPlayer)
        {
            var spawnPoint = FindSpawnPoint(spawningPlayer.playerIndex);
            if (spawnPoint == null)
                return;

            spawningPlayer.transform.position = spawnPoint.position;
            PlayerSpawned!.Invoke(spawningPlayer.gameObject);
        }

        [CanBeNull]
        private static Transform FindSpawnPoint(int playerIndex)
        {
            var spawnArea = FindObjectOfType<SpawnArea>();
            if (spawnArea == null)
                return default;

            return playerIndex switch
            {
                0 => spawnArea.Player1Spawn,
                1 => spawnArea.Player2Spawn,
                2 => spawnArea.Player3Spawn,
                3 => spawnArea.Player4Spawn,
                _ => default
            };
        }
    }
}
