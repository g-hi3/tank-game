using UnityEngine;

namespace TankGame.Core.Bomb
{
    /// <summary>
    /// Contains data that can be used to produce bombs.
    /// </summary>
    [CreateAssetMenu(menuName = "TankGame/Bomb Blueprint")]
    public class BombBlueprint : ScriptableObject
    {
        /// <summary>
        /// Represents how a bomb will be resized when it detonates.
        /// </summary>
        /// <remarks>
        /// Both dimensions of a bomb can be scaled independently. This allows for different explosion shapes (think
        /// Bomber Man). If the scale is between 0 and 1, it will shrink the explosion. This effect can be used if a
        /// bomb "implodes" and leaves a danger zone. When the scale is negative, it effectively flips the explosion.
        /// </remarks>
        [field: SerializeField] public Vector2 ExplosionScale { get; private set; }

        /// <summary>
        /// Prefab ob the bomb to use.
        /// </summary>
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}
