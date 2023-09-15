using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankGame.Core.Spawn
{
    public class PlayerSpawner : MonoBehaviour
    {
        [field: SerializeField] public PlayerInputManager PlayerInputManager { get; private set; }
        [field: SerializeField] public UnityEvent<GameObject> PlayerSpawned { get; private set; }

        private void Awake()
        {
            PlayerInputManager = GetComponent<PlayerInputManager>();
        }

        private void Start()
        {
            PlayerInputManager.onPlayerJoined += SpawnPlayer;
        }

        private void SpawnPlayer(PlayerInput spawningPlayer)
        {
            var spawnPoint = GetSpawnPoint(spawningPlayer.playerIndex);
            if (spawnPoint == null)
                return;

            spawningPlayer.transform.position = spawnPoint.position;
            PlayerSpawned.Invoke(spawningPlayer.gameObject);
        }

        private static Transform GetSpawnPoint(int playerIndex)
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
