using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuViewModel : MonoBehaviour
    {
        [NotNull]
        [field: SerializeField]
        public UIDocument DocumentRoot { get; private set; } = default!;

        [field: SerializeField] public string GameStartSceneName { get; private set; }
        [field: SerializeField] public string BestTimesSceneName { get; private set; }

        private void OnStartGameButtonClicked()
        {
            SceneManager.LoadSceneAsync(GameStartSceneName);
        }

        private void OnBestTimesButtonClicked()
        {
            SceneManager.LoadSceneAsync(BestTimesSceneName);
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
            var startGameButton = DocumentRoot.rootVisualElement.Q<Button>("StartGameButton")!;
            startGameButton.clicked += OnStartGameButtonClicked;

            var bestTimesButton = DocumentRoot.rootVisualElement.Q<Button>("BestTimesButton")!;
            bestTimesButton.clicked += OnBestTimesButtonClicked;

            var quitButton = DocumentRoot.rootVisualElement.Q<Button>("QuitButton")!;
            quitButton.clicked += OnQuitButtonClicked;
        }
    }
}