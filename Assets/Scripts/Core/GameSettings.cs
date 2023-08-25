using UnityEngine;

namespace TankGame.Core
{
    [CreateAssetMenu(menuName = "TankGame/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [field: SerializeField] public uint InitialLivesCount { get; set; }
    }
}
