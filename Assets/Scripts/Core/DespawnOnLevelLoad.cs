using UnityEngine;

namespace TankGame.Core
{
    /// <summary>
    /// This component de-spawns its game object when a level is loaded.
    /// </summary>
    public class DespawnOnLevelLoad : MonoBehaviour
    {
        private void Start() => GameManager.Instance.LevelLoaded.AddListener(OnLevelLoaded);
        private void OnLevelLoaded(LevelLoadedEventArgs unused) => Destroy(gameObject);
        private void OnDestroy() => GameManager.Instance.LevelLoaded.RemoveListener(OnLevelLoaded);
    }
}