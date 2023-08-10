using UnityEngine;

namespace TankGame.Core
{
    public class BombBlueprint : ScriptableObject
    {
        [field: SerializeField] public Vector2 ExplosionScale { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}
