using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Core.Save;
using TankGame.Core.Spawn;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        [field: SerializeField] public bool Paused { get; private set; }

        public void PauseGame()
        {
            Paused = true;
            FindObjectOfType<LevelTimer>()?.Pause();
            GamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            Paused = false;
            FindObjectOfType<LevelTimer>()?.Resume();
            GameResumed?.Invoke();
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
                    if (LevelSceneNames[i] != _currentLevelSceneName)
                        continue;

                    if (i < LevelSceneNames.Length - 1)
                    {
                        CreateTimerStep();
                        var levelSceneName = LevelSceneNames[i + 1];
                        StartCoroutine(WaitThenDo(() => LoadLevel(levelSceneName)));
                    }
                    else
                    {
                        // TODO: This is the end of the game, show times.
                        CreateTimerStep();
                        SaveAttempt();
                    }
                }
            }
        }

        private static IEnumerator<WaitForSeconds> WaitThenDo(Action action)
        {
            yield return new WaitForSeconds(5f);
            action.Invoke();
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

        private void SaveAttempt()
        {
            var levelTimer = FindObjectOfType<LevelTimer>();
            if (levelTimer == null)
                return;

            var steps = LevelSceneNames
                .Zip(
                    levelTimer.GetSteps(),
                    (level, time) => new LevelTime(level, time));
            var saveData = GameSaveData.Load();
            var attempts = saveData.Attempts.ToList();
            attempts.Add(new Attempt(steps));
            saveData.Attempts = attempts.ToArray();
            GameSaveData.Save(saveData);
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

        public void OnPlayerSpawned(GameObject playerObject)
        {
            var tank = playerObject.GetComponent<Tank>();
            tank.Eliminated.AddListener(OnTankEliminated);
        }

        private void LoadLevel(string levelSceneName)
        {
            // TODO: Make sure to place all active player objects at the designated spawn area of the loaded level.
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
            loadSceneTask.completed += _ => MovePlayersToSpawn();
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

        private static void MovePlayersToSpawn()
        {
            var spawnArea = FindObjectOfType<SpawnArea>();
            spawnArea.ResetSpawns();
            var playerTanks = FindObjectsOfType<PlayerInput>();

            foreach (var playerTank in playerTanks)
            {
                var playerObject = playerTank.gameObject;
                var playerTransform = playerTank.transform;

                if (playerObject.layer == LayerMask.NameToLayer("Player 1 Objects"))
                    playerTransform.position = spawnArea.Player1Spawn.position;
                else if (playerObject.layer == LayerMask.NameToLayer("Player 2 Objects"))
                    playerTransform.position = spawnArea.Player2Spawn.position;
                else if (playerObject.layer == LayerMask.NameToLayer("Player 3 Objects"))
                    playerTransform.position = spawnArea.Player3Spawn.position;
                else if (playerObject.layer == LayerMask.NameToLayer("Player 4 Objects"))
                    playerTransform.position = spawnArea.Player4Spawn.position;
            }
        }
    }
}