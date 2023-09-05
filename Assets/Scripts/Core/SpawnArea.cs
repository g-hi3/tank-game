using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankGame.Core
{
    public class SpawnArea : MonoBehaviour
    {
        private IEnumerable<Transform> ChildTransforms =>
            Enumerable
                .Range(0, transform.childCount)
                .Select(childIndex => transform.GetChild(childIndex));

        private void Start()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawner>();

            foreach (var childTransform in ChildTransforms)
            {
                playerSpawner.RegisterSpawnPoint(childTransform);
            }
        }

        private void OnDestroy()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawner>();

            foreach (var childTransform in ChildTransforms)
            {
                playerSpawner.UnregisterSpawnPoint(childTransform);
            }
        }
    }
}