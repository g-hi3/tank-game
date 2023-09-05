using UnityEngine;

namespace TankGame.Core
{
    public class SpawnArea : MonoBehaviour
    {
        private void Start()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawner>();

            for (var i = 0; i < transform.childCount; i++)
            {
                var childObject = transform.GetChild(i);
                playerSpawner.RegisterSpawnPoint(childObject);
            }
        }
    }
}