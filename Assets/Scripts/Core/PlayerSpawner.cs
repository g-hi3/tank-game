using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Core
{
    public class PlayerSpawner : MonoBehaviour
    {
        [field: SerializeField] public List<Transform> SpawnPoints { get; private set; }
        [field: SerializeField] public GameManager GameManager { get; private set; }
        private PlayerInputManager _playerInputManager;

        public void RegisterSpawnPoint(Transform spawnPoint)
        {
            SpawnPoints.Add(spawnPoint);
        }

        public void UnregisterSpawnPoint(Transform childObject)
        {
            SpawnPoints.Remove(childObject);
        }

        private void Awake()
        {
            _playerInputManager = GetComponent<PlayerInputManager>();
        }

        private void Start()
        {
            GameManager = GameManager.Instance;
            _playerInputManager.onPlayerJoined += SpawnPlayer;
        }

        private void SpawnPlayer(PlayerInput spawningPlayer)
        {
            var playerIndex = spawningPlayer.playerIndex;
            var spawnPoint = SpawnPoints[playerIndex];
            var playerTransform = spawningPlayer.transform;
            playerTransform.position = spawnPoint.position;
            GameManager.RegisterTank(spawningPlayer.GetComponent<Tank>());
        }
    }
}
