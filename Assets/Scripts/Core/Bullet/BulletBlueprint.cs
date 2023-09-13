using UnityEngine;

namespace TankGame.Core.Bullet
{
    [CreateAssetMenu(menuName = "TankGame/Bullet Blueprint")]
    public class BulletBlueprint : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public uint RicochetCount { get; private set; }
    }
}
