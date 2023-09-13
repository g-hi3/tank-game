using UnityEngine;

namespace TankGame.Core.Bomb
{
    public class BombFactory : MonoBehaviour
    {
        [field: SerializeField] public BombBlueprint Blueprint { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }

        public GameObject Make(Transform spawn)
        {
            var bombObject = Instantiate(Prefab, spawn.position, spawn.rotation);
            _ = Bomb.FromBlueprint(Blueprint, bombObject);
            return bombObject;
        }
    }
}
