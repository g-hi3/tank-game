using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    /// <summary>
    /// Handles the UI of the main menu.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuViewModel : MonoBehaviour
    {
        private VisualElement _creditsContainer;

        /// <summary>
        /// This component controls the UI.
        /// </summary>
        [NotNull]
        [field: SerializeField]
        public UIDocument DocumentRoot { get; private set; } = default!;

        /// <summary>
        /// Represents the name of the scene that starts the game.
        /// </summary>
        /// <remarks>
        /// This is not the name of level 1. Instead, it's the common scene that loads levels individually.
        /// </remarks>
        [field: SerializeField] public string GameStartSceneName { get; private set; }

        /// <summary>
        /// Represents the name of the scene that shows the best times.
        /// </summary>
        [field: SerializeField] public string BestTimesSceneName { get; private set; }

        private void OnStartGameButtonClicked()
        {
            SceneManager.LoadSceneAsync(GameStartSceneName);
        }

        private void OnBestTimesButtonClicked()
        {
            SceneManager.LoadSceneAsync(BestTimesSceneName);
        }

        private void OnCreditsButtonClicked()
        {
            if (_creditsContainer is { style: { } creditsStyle })
                creditsStyle.display = creditsStyle.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>()
                           ?? throw new InvalidOperationException($"Missing {nameof(UIDocument)} component!");
        }

        private void Start()
        {
            _creditsContainer = DocumentRoot.rootVisualElement.Q<VisualElement>("CreditsContainer");

            if (_creditsContainer is { style: not null })
                _creditsContainer.style.display = DisplayStyle.None;

            var startGameButton = DocumentRoot.rootVisualElement.Q<Button>("StartGameButton")!;
            startGameButton.clicked += OnStartGameButtonClicked;

            var bestTimesButton = DocumentRoot.rootVisualElement.Q<Button>("BestTimesButton")!;
            bestTimesButton.clicked += OnBestTimesButtonClicked;

            var creditsButton = DocumentRoot.rootVisualElement.Q<Button>("CreditsButton")!;
            creditsButton.clicked += OnCreditsButtonClicked;

            var quitButton = DocumentRoot.rootVisualElement.Q<Button>("QuitButton")!;
            quitButton.clicked += OnQuitButtonClicked;
        }
    }
}