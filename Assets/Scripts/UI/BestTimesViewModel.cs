using System;
using System.Linq;
using JetBrains.Annotations;
using TankGame.Core.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    /// <summary>
    /// Handles the UI of the best times menu.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class BestTimesViewModel : MonoBehaviour
    {
        /// <summary>
        /// This component controls the UI.
        /// </summary>
        [NotNull]
        [field: SerializeField]
        public UIDocument DocumentRoot { get; private set; } = default!;

        /// <summary>
        /// Contains the name of the main menu scene.
        /// </summary>
        [field: SerializeField] public string MainMenuSceneName { get; private set; }

        private void OnMainMenuButtonClicked()
        {
            SceneManager.LoadSceneAsync(MainMenuSceneName);
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>()
                           ?? throw new InvalidOperationException($"Missing {nameof(UIDocument)} component!");
        }

        private void Start()
        {
            var mainMenuButton = DocumentRoot!.rootVisualElement.Q<Button>("MainMenuButton")!;
            mainMenuButton.clicked += OnMainMenuButtonClicked;

            var gameSaveData = GameSaveData.Load();
            var bestTimesContainer = DocumentRoot.rootVisualElement.Q<ScrollView>("best-times-container");
            foreach (var attempt in gameSaveData.attempts
                         .OrderByDescending(attempt => attempt.levelTimes.Length)
                         .ThenBy(attempt => attempt.levelTimes.Sum(levelTime => levelTime.time.Ticks)))
            {
                var labelText = string.Join("\t", attempt.levelTimes.Select(levelTime => $"{levelTime.name}: {levelTime.time:g}"));
                var bestTimeAttempt = new Label(labelText);
                bestTimeAttempt.AddToClassList("best-time-attempt");
                var container = new VisualElement { style = { alignItems = Align.Center } };
                container.Add(bestTimeAttempt);
                bestTimesContainer.Add(container);
            }
        }
    }
}