using JetBrains.Annotations;
using TankGame.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    public class GameUIViewModel : MonoBehaviour
    {
        private const string TimerFormat = "{0:D2}:{1:D2}.{2:D3}";
        private const string LivesFormat = "{0}";
        private Label _levelName;
        private Label _speedRunTimer;
        private Label _remainingLives;
        private VisualElement _pauseMenu;

        [field: SerializeField] public UIDocument DocumentRoot { get; private set; }
        [field: SerializeField] public LevelTimer Timer { get; private set; }
        [field: SerializeField] public GameManager GameManager { get; private set; }

        public void ShowGameOver()
        {
            var gameOverScreen = DocumentRoot.rootVisualElement.Q("GameOverScreen");
            gameOverScreen.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        private void ShowRemainingLives()
        {
            _remainingLives.text = string.Format(LivesFormat, GameManager.RemainingLives);
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>();
            _levelName = DocumentRoot.rootVisualElement.Q<Label>("LevelName");
            _speedRunTimer = DocumentRoot.rootVisualElement.Q<Label>("SpeedRunTimer");
            _remainingLives = DocumentRoot.rootVisualElement.Q<Label>("RemainingLives");
            _pauseMenu = DocumentRoot.rootVisualElement.Q("PauseMenu");
        }

        private void Start()
        {
            GameManager = GameManager.Instance;
            GameManager.PlayerTankEliminated.AddListener(_ => ShowRemainingLives());
            ShowRemainingLives();

            var resumeButton = DocumentRoot!.rootVisualElement.Q<Button>("ResumeButton")!;
            resumeButton.clicked += OnResumeButtonClicked;

            var mainMenuButton = DocumentRoot.rootVisualElement.Q<Button>("MainMenuButton")!;
            mainMenuButton.clicked += OnMainMenuButtonClicked;

            var mainMenuButton2 = DocumentRoot.rootVisualElement.Q<Button>("MainMenuButton2")!;
            mainMenuButton2.clicked += OnMainMenuButtonClicked;
        }

        private void Update()
        {
            _speedRunTimer.text = _speedRunTimer.text = string.Format(
                TimerFormat,
                Timer.Minutes,
                Timer.Seconds,
                Timer.Milliseconds);
        }

        [UsedImplicitly]
        private void OnPause()
        {
            _pauseMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        [UsedImplicitly]
        private void OnResume()
        {
            _pauseMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private void OnResumeButtonClicked()
        {
            GameManager.ResumeGame();
        }

        private static void OnMainMenuButtonClicked()
        {
            GameManager.GoToMainMenu();
        }

        public void OnLevelLoaded(LevelLoadedEventArgs eventArgs)
        {
            _levelName.text = eventArgs.LevelName;
        }
    }
}