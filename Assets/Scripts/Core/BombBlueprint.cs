using UnityEngine;

namespace TankGame.Core
{
    [CreateAssetMenu(menuName = "TankGame/Bomb Blueprint")]
    public class BombBlueprint : ScriptableObject
    {
        [field: SerializeField] public Vector2 ExplosionScale { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}
