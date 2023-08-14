using System.Collections.Generic;
using TankGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    private PlayerInputManager _playerInputManager;

    private void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        _playerInputManager.onPlayerJoined += SpawnPlayer;
    }

    private void SpawnPlayer(PlayerInput spawningPlayer)
    {
        var playerIndex = spawningPlayer.playerIndex;
        var spawnPoint = spawnPoints[playerIndex];
        var playerTransform = spawningPlayer.transform;
        playerTransform.position = spawnPoint.position;
        GameManager.Instance.RegisterTank(spawningPlayer.GetComponent<Tank>());
    }
}
