using UnityEngine;

namespace TankGame.Core
{
    public class BombFactory : MonoBehaviour
    {
        [field: SerializeField] public BombBlueprint Blueprint { get; private set; }

        public Bomb Make()
        {
            return Bomb.FromBlueprint(Blueprint);
        }
    }
}
