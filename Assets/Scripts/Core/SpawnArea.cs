using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankGame.Core
{
    public class SpawnArea : MonoBehaviour
    {
        [field: SerializeField] public Transform Player1Spawn { get; private set; }
        [field: SerializeField] public Transform Player2Spawn { get; private set; }
        [field: SerializeField] public Transform Player3Spawn { get; private set; }
        [field: SerializeField] public Transform Player4Spawn { get; private set; }

        private IEnumerable<Transform> ChildTransforms =>
            Enumerable
                .Range(0, transform.childCount)
                .Select(childIndex => transform.GetChild(childIndex));

        private void Start()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawner>();

            foreach (var childTransform in ChildTransforms)
                playerSpawner.RegisterSpawnPoint(childTransform);
        }

        public void ResetSpawns()
        {
            var playerSpawns = FindObjectsOfType<PlayerSpawn>();
            Player1Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 1 Objects");
            Player2Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 2 Objects");
            Player3Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 3 Objects");
            Player4Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 4 Objects");
        }

        private static Transform ExtractSpawnPointTransform(IEnumerable<PlayerSpawn> playerSpawns, string layerName)
        {
            return playerSpawns
                .Single(playerSpawn => playerSpawn.gameObject.layer == LayerMask.NameToLayer(layerName))
                .GetComponent<Transform>();
        }

        private void OnDestroy()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawner>();
            if (playerSpawner == null)
                return;

            foreach (var childTransform in ChildTransforms)
                playerSpawner.UnregisterSpawnPoint(childTransform);
        }
    }
}