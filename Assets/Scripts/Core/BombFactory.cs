using UnityEngine;

namespace TankGame.Core
{
    public class BombFactory : MonoBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }

        public Bomb Make(BombBlueprint blueprint)
        {
            return Bomb.FromBlueprint(blueprint);
        }
    }
}
