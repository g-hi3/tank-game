using JetBrains.Annotations;
using UnityEngine;

namespace TankGame.Core.Bullet
{
    /// <summary>
    /// Produces game objects with a <see cref="Bullet"/> component.
    /// </summary>
    public class BulletFactory : MonoBehaviour
    {
        /// <summary>
        /// This blueprint is used to set data on the newly created component.
        /// </summary>
        [field: SerializeField] public BulletBlueprint Blueprint { get; private set; }

        /// <summary>
        /// This prefab is used to instantiate the game object.
        /// </summary>
        [field: SerializeField] public GameObject Prefab { get; private set; }

        /// <summary>
        /// Creates a new game object using <see cref="Prefab"/>.
        /// </summary>
        /// <param name="spawn">this transform is used for the position and rotation of the bullet</param>
        /// <returns></returns>
        public GameObject Make([NotNull] Transform spawn)
        {
            var spawnPosition = spawn.position;
            var bulletDirection = spawnPosition - transform.position;
            var bulletObject = Instantiate(Prefab, spawnPosition, spawn.rotation)!;
            _ = Bullet.FromBlueprint(Blueprint, bulletDirection, bulletObject);
            return bulletObject;
        }
    }
}