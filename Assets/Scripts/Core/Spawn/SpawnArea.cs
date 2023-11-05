using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.Spawn
{
    /// <summary>
    /// Manages spawn areas for player tanks.
    /// </summary>
    public class SpawnArea : MonoBehaviour
    {
        /// <summary>
        /// Spawn position and rotation for player 1.
        /// </summary>
        [field: SerializeField] public Transform Player1Spawn { get; private set; }

        /// <summary>
        /// Spawn position and rotation for player 2.
        /// </summary>
        [field: SerializeField] public Transform Player2Spawn { get; private set; }

        /// <summary>
        /// Spawn position and rotation for player 3.
        /// </summary>
        [field: SerializeField] public Transform Player3Spawn { get; private set; }

        /// <summary>
        /// Spawn position and rotation for player 4.
        /// </summary>
        [field: SerializeField] public Transform Player4Spawn { get; private set; }

        /// <summary>
        /// Find spawns in scene using the "Respawn" tag and layers "Player x Objects" where x is a number between 1 and
        /// 4.
        /// </summary>
        public void ResetSpawns()
        {
            var playerSpawns = FindObjectsOfType<Transform>()?
                .Where(obj => obj && obj.CompareTag("Respawn"))
                .ToArray()
                ?? Array.Empty<Transform>();
            Player1Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 1 Objects");
            Player2Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 2 Objects");
            Player3Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 3 Objects");
            Player4Spawn = ExtractSpawnPointTransform(playerSpawns, "Player 4 Objects");
        }

        private static Transform ExtractSpawnPointTransform(
            [NotNull] IEnumerable<Transform> playerSpawns,
            string layerName)
        {
            return playerSpawns
                .Where(playerSpawn => playerSpawn != null)
                .Single(playerSpawn => playerSpawn.gameObject.layer == LayerMask.NameToLayer(layerName))
                .GetComponent<Transform>();
        }
    }
}