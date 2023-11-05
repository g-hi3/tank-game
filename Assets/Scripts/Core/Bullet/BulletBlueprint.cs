using UnityEngine;

namespace TankGame.Core.Bullet
{
    /// <summary>
    /// Contains data for creating <see cref="Bullet"/> components.
    /// </summary>
    [CreateAssetMenu(menuName = "TankGame/Bullet Blueprint")]
    public class BulletBlueprint : ScriptableObject
    {
        /// <summary>
        /// Represents the speed of the bullet.
        /// </summary>
        [field: SerializeField] public float Speed { get; private set; }

        /// <summary>
        /// Represents the maximum number times the bullet can ricochet before being destroyed.
        /// </summary>
        [field: SerializeField] public uint RicochetCount { get; private set; }
    }
}
