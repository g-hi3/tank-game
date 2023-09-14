using UnityEngine;

namespace TankGame.Core
{
    public class Pausable : MonoBehaviour
    {
        private void SendPause() => SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);

        private void SendResume() => SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);

        private void Start()
        {
            GameManager.Instance.GamePaused.AddListener(SendPause);
            GameManager.Instance.GameResumed.AddListener(SendResume);
        }

        private void OnDestroy()
        {
            GameManager.Instance.GamePaused.RemoveListener(SendPause);
            GameManager.Instance.GameResumed.RemoveListener(SendResume);
        }
    }
}