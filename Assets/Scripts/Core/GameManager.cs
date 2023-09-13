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
            FindObjectOfType<LevelTimer>()?.Pause();
            GamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            FindObjectOfType<LevelTimer>()?.Resume();
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
                RemainingLives--;
                if (RemainingLives > 0)
                {
                    PlayerTankEliminated?.Invoke(new PlayerTankEliminatedEventArgs(RemainingLives));
                    ResetTimerStep();
                    ResetLevel();
                }
                else
                {
                    // TODO: Show final times and save them.
                    CreateTimerStep();
                    SceneManager.LoadScene("Main Menu");
                }
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
                        {
                            CreateTimerStep();
                            // TODO: Wait a couple of seconds here to let the confetti play before loading the next scen.
                            LoadLevel(LevelSceneNames[i + 1]);
                        }
                        else
                        {
                            // TODO: This is the end of the game, show times.
                            CreateTimerStep();
                        }
                    }
                }
            }
        }

        private static void ResetTimerStep()
        {
            var levelTimer = FindObjectOfType<LevelTimer>()!;
            levelTimer.Pause();
            levelTimer.ResetStep();
        }

        private static void CreateTimerStep()
        {
            var levelTimer = FindObjectOfType<LevelTimer>()!;
            levelTimer.Pause();
            levelTimer.CreateStep();
        }

        private void Awake()
        {
            Instance ??= this;
            RemainingLives = Settings.InitialLivesCount;

            LoadLevel(LevelSceneNames[0]);
        }

        private void Start()
        {
            FindObjectOfType<LevelTimer>()?.Resume();
        }

        private void LoadLevel(string levelSceneName)
        {
            var currentLevelScene = SceneManager.GetSceneByName(_currentLevelSceneName);
            if (currentLevelScene.isLoaded)
                _ = SceneManager.UnloadSceneAsync(currentLevelScene);

            _currentLevelSceneName = levelSceneName;
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            var loadSceneTask = SceneManager.LoadSceneAsync(_currentLevelSceneName, LoadSceneMode.Additive);

            loadSceneTask.completed += _ => FindEnemyTanks();
            loadSceneTask.completed += _ => LevelLoaded?.Invoke(new LevelLoadedEventArgs(_currentLevelSceneName));
        }

        private void ResetLevel()
        {
            var currentLevelScene = SceneManager.GetSceneByName(_currentLevelSceneName);
            if (currentLevelScene.isLoaded)
            {
                var asyncOperation = SceneManager.UnloadSceneAsync(currentLevelScene);
                asyncOperation.completed += _ => LoadCurrentLevel();
            }
            else
            {
                LoadCurrentLevel();
            }
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