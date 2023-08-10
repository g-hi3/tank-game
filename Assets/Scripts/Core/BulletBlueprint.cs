using UnityEngine;

namespace TankGame.Core
{
    public class BulletBlueprint : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public uint RicochetCount { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}
