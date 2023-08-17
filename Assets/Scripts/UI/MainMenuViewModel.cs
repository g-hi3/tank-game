using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    public class MainMenuViewModel : MonoBehaviour
    {
        [field: SerializeField] public UIDocument DocumentRoot { get; private set; }
        [field: SerializeField] public string Level1SceneName { get; private set; }
        [field: SerializeField] public string BestTimesSceneName { get; private set; }

        private void OnStartGameButtonClicked()
        {
            SceneManager.LoadSceneAsync(Level1SceneName);
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
            DocumentRoot = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var startGameButton = DocumentRoot!.rootVisualElement.Q<Button>("StartGameButton")!;
            startGameButton.clicked += OnStartGameButtonClicked;

            var bestTimesButton = DocumentRoot.rootVisualElement.Q<Button>("BestTimesButton")!;
            bestTimesButton.clicked += OnBestTimesButtonClicked;

            var quitButton = DocumentRoot.rootVisualElement.Q<Button>("QuitButton")!;
            quitButton.clicked += OnQuitButtonClicked;
        }
    }
}