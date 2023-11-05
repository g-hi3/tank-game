using UnityEngine;

namespace TankGame.Core
{
    /// <summary>
    /// Contains various settings about the game.
    /// </summary>
    [CreateAssetMenu(menuName = "TankGame/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// Represents the number of lives a player gets at the start of the game.
        /// </summary>
        [field: SerializeField] public uint InitialLivesCount { get; set; }
    }
}
