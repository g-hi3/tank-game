using TankGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    public class GameUIViewModel : MonoBehaviour
    {
        private const string TimerFormat = "{0:D2}:{1:D2}.{2:D3}";
        private const string LivesFormat = "{0:D2}";
        private Label _levelName;
        private Label _speedRunTimer;
        private Label _remainingLives;

        [field: SerializeField] public UIDocument DocumentRoot { get; private set; }
        [field: SerializeField] public LevelTimer Timer { get; private set; }
        [field: SerializeField] public GameManager GameManager { get; private set; }

        private void ShowLevelName()
        {
            var activeScene = SceneManager.GetActiveScene();
            _levelName.text = activeScene.name;
        }

        private void ShowRemainingLives()
        {
            _remainingLives.text = string.Format(LivesFormat, GameManager.RemainingLives);
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _levelName = DocumentRoot.rootVisualElement.Q<Label>("LevelName");
            _speedRunTimer = DocumentRoot.rootVisualElement.Q<Label>("SpeedRunTimer");
            _remainingLives = DocumentRoot.rootVisualElement.Q<Label>("RemainingLives");
            ShowLevelName();
            ShowRemainingLives();
        }

        private void Update()
        {
            _speedRunTimer.text = _speedRunTimer.text = string.Format(
                TimerFormat,
                Timer.Minutes,
                Timer.Seconds,
                Timer.Milliseconds);
        }
    }
}