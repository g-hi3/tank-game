using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TankGame.UI
{
    public class BestTimesViewModel : MonoBehaviour
    {
        [field: SerializeField] public UIDocument DocumentRoot { get; private set; }
        [field: SerializeField] public string MainMenuSceneName { get; private set; }

        private void OnMainMenuButtonClicked()
        {
            SceneManager.LoadSceneAsync(MainMenuSceneName);
        }

        private void Awake()
        {
            DocumentRoot = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var mainMenuButton = DocumentRoot!.rootVisualElement.Q<Button>("MainMenuButton")!;
            mainMenuButton.clicked += OnMainMenuButtonClicked;
        }
    }
}