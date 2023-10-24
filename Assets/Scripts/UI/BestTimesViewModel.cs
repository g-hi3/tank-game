using System;
using System.Linq;
using JetBrains.Annotations;
using TankGame.Core.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class BestTimesViewModel : MonoBehaviour
    {
        [NotNull]
        [field: SerializeField]
        public UIDocument DocumentRoot { get; private set; } = default!;

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
            foreach (var attempt in gameSaveData.Attempts)
            {
                bestTimesContainer.Add(new Label
                {
                    text = string.Join("\t", attempt.LevelTimes.Select(levelTime => $"{levelTime.Name}: {levelTime.Time}"))
                });
            }
        }
    }
}