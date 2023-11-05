using UnityEngine;

namespace TankGame.Core.Bomb
{
    /// <summary>
    /// Produces game objects with a <see cref="Bomb"/> component.
    /// </summary>
    public class BombFactory : MonoBehaviour
    {
        /// <summary>
        /// The factory uses this when creating the <see cref="Bomb"/> component.
        /// </summary>
        [field: SerializeField] public BombBlueprint Blueprint { get; private set; }

        /// <summary>
        /// The factory uses this when instantiating a new game object.
        /// </summary>
        [field: SerializeField] public GameObject Prefab { get; private set; }

        /// <summary>
        /// Instantiates a new game object with a <see cref="Bomb"/> component from <see cref="Prefab"/>.
        /// </summary>
        /// <param name="spawn">transform to use position and rotation for spawn</param>
        /// <returns>a new game object with a <see cref="Bomb"/> component</returns>
        public GameObject Make(Transform spawn)
        {
            var bombObject = Instantiate(Prefab, spawn.position, spawn.rotation);
            _ = Bomb.FromBlueprint(Blueprint, bombObject);
            return bombObject;
        }
    }
}
