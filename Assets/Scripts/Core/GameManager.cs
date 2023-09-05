using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TankGame.Core
{
    public class GameManager : MonoBehaviour
    {
        private string _currentLevelSceneName;

        public static GameManager Instance { get; private set; }

        [field: SerializeField] public GameSettings Settings { get; private set; }
        [field: SerializeField] public uint RemainingLives { get; private set; }
        [field: SerializeField] public Tank[] RemainingEnemyTanks { get; private set; }
        [field: SerializeField] public UnityEvent<PlayerTankEliminatedEventArgs> PlayerTankEliminated { get; private set; }
        [field: SerializeField] public UnityEvent AllEnemyTanksEliminated { get; private set; }
        [field: SerializeField] public UnityEvent GamePaused { get; private set; }
        [field: SerializeField] public UnityEvent GameResumed { get; private set; }
        [field: SerializeField] public UnityEvent<LevelLoadedEventArgs> LevelLoaded { get; private set; }
        [field: SerializeField] public string[] LevelSceneNames { get; private set; }

        public void PauseGame()
        {
            GamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            GameResumed?.Invoke();
        }

        public void RegisterTank(Tank tank)
        {
            tank.Eliminated.AddListener(OnTankEliminated);
        }

        private void OnTankEliminated(Tank tank)
        {
            if (tank.CompareTag("Player"))
            {
                PlayerTankEliminated?.Invoke(new PlayerTankEliminatedEventArgs(RemainingLives));
            }
            else if (RemainingEnemyTanks.All(
                         enemyTank => enemyTank == null
                                      || !enemyTank.CompareTag("Player")
                                      || enemyTank.gameObject == null))
            {
                AllEnemyTanksEliminated?.Invoke();

                for (var i = 0; i < LevelSceneNames.Length; i++)
                {
                    if (LevelSceneNames[i] == _currentLevelSceneName)
                    {
                        if (i < LevelSceneNames.Length - 1)
                            LoadLevel(LevelSceneNames[i + 1]);
                        else
                        {
                            // TODO: This is the end of the game, show times.
                        }
                    }
                }
            }
        }

        private void Awake()
        {
            Instance ??= this;
        }

        private void Start()
        {
            RemainingLives = Settings.InitialLivesCount;

            LoadLevel(LevelSceneNames[0]);
        }

        private void LoadLevel(string levelSceneName)
        {
            var currentLevelScene = SceneManager.GetSceneByName(_currentLevelSceneName);
            if (currentLevelScene.isLoaded)
                _ = SceneManager.UnloadSceneAsync(currentLevelScene);

            _currentLevelSceneName = levelSceneName;
            var loadSceneTask = SceneManager.LoadSceneAsync(_currentLevelSceneName, LoadSceneMode.Additive);

            loadSceneTask.completed += _ => FindEnemyTanks();
            loadSceneTask.completed += _ => LevelLoaded?.Invoke(new LevelLoadedEventArgs(levelSceneName));
            // TODO: When new scene has loaded, update level name.
            // TODO: When new scene has loaded, update spawn areas.
        }

        private void FindEnemyTanks()
        {
            foreach (var tank in RemainingEnemyTanks)
                tank?.Eliminated.RemoveListener(OnTankEliminated);

            RemainingEnemyTanks = FindObjectsOfType<Tank>()
                .Where(tank => !tank.CompareTag("Player"))
                .ToArray();

            foreach (var tank in RemainingEnemyTanks)
                tank.Eliminated.AddListener(OnTankEliminated);
        }
    }
}