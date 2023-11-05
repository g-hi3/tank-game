using System;
using JetBrains.Annotations;
using TankGame.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    /// <summary>
    /// Handles the UI of the game.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class GameUIViewModel : MonoBehaviour
    {
        private const string TimerFormat = "{0:D2}:{1:D2}.{2:D3}";
        private const string LivesFormat = "{0}";
        private Label _levelName;
        private Label _speedRunTimer;
        private Label _remainingLives;
        private VisualElement _pauseMenu;

        /// <summary>
        /// This component controls the UI.
        /// </summary>
        [NotNull]
        [field: SerializeField]
        public UIDocument DocumentRoot { get; private set; } = default!;

        /// <summary>
        /// This property contains the level timer.
        /// </summary>
        [field: SerializeField] public LevelTimer Timer { get; private set; }

        /// <summary>
        /// This component is the main game manager.
        /// </summary>
        [field: SerializeField] public GameManager GameManager { get; private set; }

        /// <summary>
        /// Displays the game over screen.
        /// </summary>
        public void ShowGameOver()
        {
            var gameOverScreen = DocumentRoot.rootVisualElement.Q("GameOverScreen");
            if (gameOverScreen is { style: not null })
                gameOverScreen.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        private void ShowRemainingLives()
        {
            if (GameManager && _remainingLives != null)
                _remainingLives.text = string.Format(LivesFormat, GameManager.RemainingLives);
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>()
                           ?? throw new InvalidOperationException($"Missing {nameof(UIDocument)} component!");
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

            var resumeButton = DocumentRoot.rootVisualElement.Q<Button>("ResumeButton")!;
            resumeButton.clicked += OnResumeButtonClicked;

            var mainMenuButton = DocumentRoot.rootVisualElement.Q<Button>("MainMenuButton")!;
            mainMenuButton.clicked += OnMainMenuButtonClicked;

            var mainMenuButton2 = DocumentRoot.rootVisualElement.Q<Button>("MainMenuButton2")!;
            mainMenuButton2.clicked += OnMainMenuButtonClicked;
        }

        private void Update()
        {
            if (Timer && _speedRunTimer != null)
                _speedRunTimer.text = _speedRunTimer.text = string.Format(
                    TimerFormat,
                    Timer.Minutes,
                    Timer.Seconds,
                    Timer.Milliseconds);
        }

        [UsedImplicitly]
        private void OnPause()
        {
            if (_pauseMenu is { style: not null })
                _pauseMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        [UsedImplicitly]
        private void OnResume()
        {
            if (_pauseMenu is { style: not null })
                _pauseMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private void OnResumeButtonClicked()
        {
            if (GameManager)
                GameManager.ResumeGame();
        }

        private static void OnMainMenuButtonClicked()
        {
            GameManager.GoToMainMenu();
        }

        /// <summary>
        /// Updates the level name on the UI.
        /// </summary>
        /// <param name="eventArgs"></param>
        public void OnLevelLoaded(LevelLoadedEventArgs eventArgs)
        {
            if (_levelName != null)
                _levelName.text = eventArgs.LevelName;
        }
    }
}