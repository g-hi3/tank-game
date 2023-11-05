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
    /// <summary>
    /// This component controls the game flow.
    /// </summary>
    /// <remarks>
    /// There should only ever be one instance of this component per scene.
    /// </remarks>
    public class GameManager : MonoBehaviour
    {
        private string _currentLevelSceneName;

        /// <summary>
        /// The main instance of this component.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Contains settings about the game, such as lives per attempt.
        /// </summary>
        [field: SerializeField] public GameSettings Settings { get; private set; }

        /// <summary>
        /// Represents the remaining lives the player has.
        /// </summary>
        [field: SerializeField] public uint RemainingLives { get; private set; }

        /// <summary>
        /// Contains all enemy tanks in the scene.
        /// </summary>
        [field: SerializeField] public Tank[] RemainingEnemyTanks { get; private set; }

        /// <summary>
        /// This event is fired whenever a player tank was eliminated.
        /// </summary>
        [field: SerializeField] public UnityEvent<PlayerTankEliminatedEventArgs> PlayerTankEliminated { get; private set; }

        /// <summary>
        /// This event is fired whenever all enemy tanks have been eliminated.
        /// </summary>
        [field: SerializeField] public UnityEvent AllEnemyTanksEliminated { get; private set; }

        /// <summary>
        /// This event is fired when the game was paused.
        /// </summary>
        [field: SerializeField] public UnityEvent GamePaused { get; private set; }

        /// <summary>
        /// This event is fired when the was resumed.
        /// </summary>
        [field: SerializeField] public UnityEvent GameResumed { get; private set; }

        /// <summary>
        /// This event is fired when the game is over.
        /// </summary>
        /// <remarks>
        /// A "game over" is either when the player has no more lives and is eliminated, or when the player wins the
        /// final level.
        /// </remarks>
        [field: SerializeField] public UnityEvent GameOver { get; private set; }

        /// <summary>
        /// This event is fired when a level is loaded.
        /// </summary>
        /// <remarks>
        /// A level may be loaded when it's started the first time, but also when all player tanks are eliminated.
        /// </remarks>
        [field: SerializeField] public UnityEvent<LevelLoadedEventArgs> LevelLoaded { get; private set; }

        /// <summary>
        /// Contains the name of all scenes that represent playable levels.
        /// </summary>
        /// <remarks>
        /// This property only contains level scenes, not menu scenes.
        /// </remarks>
        [field: SerializeField] public string[] LevelSceneNames { get; private set; }

        /// <summary>
        /// Represents the game's pause state.
        /// </summary>
        [field: SerializeField] public bool Paused { get; private set; }

        /// <summary>
        /// Contains the level timer that tracks the progress of an attempt.
        /// </summary>
        [field: SerializeField] public LevelTimer LevelTimer { get; private set; }

        /// <summary>
        /// Determines whether tanks are eliminated when they're hit by a bullet or explosion.
        /// </summary>
        public bool GameRulesActive { get; private set; }

        /// <summary>
        /// Sets the game's pause state to paused.
        /// </summary>
        public void PauseGame()
        {
            Paused = true;
            LevelTimer.Pause();
            GamePaused?.Invoke();
        }

        /// <summary>
        /// Sets the game's pause state to resumed.
        /// </summary>
        public void ResumeGame()
        {
            Paused = false;
            LevelTimer.Resume();
            GameResumed?.Invoke();
        }

        private void OnTankEliminated(Tank tank)
        {
            if (tank.CompareTag("Player"))
            {
                RemainingLives--;
                PlayerTankEliminated?.Invoke(new PlayerTankEliminatedEventArgs(RemainingLives));
                if (RemainingLives > 0)
                {
                    ResetTimerStep();
                    ResetLevel();
                }
                else
                {
                    CreateTimerStep();
                    SaveAttempt();
                    GameOver?.Invoke();
                }
            }
            // Warning: tank is destroyed next frame, so one should still be remaining.
            else if (RemainingEnemyTanks.Count(enemyTank => enemyTank != null) == 1)
            {
                AllEnemyTanksEliminated?.Invoke();

                for (var i = 0; i < LevelSceneNames.Length; i++)
                {
                    if (LevelSceneNames[i] != _currentLevelSceneName)
                        continue;

                    GameRulesActive = false;
                    if (i < LevelSceneNames.Length - 1)
                    {
                        CreateTimerStep();
                        var levelSceneName = LevelSceneNames[i + 1];
                        StartCoroutine(WaitThenDo(() => LoadLevel(levelSceneName)));
                    }
                    else
                    {
                        CreateTimerStep();
                        SaveAttempt();
                        GameOver?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the current scene and loads the main menu scene.
        /// </summary>
        public static void GoToMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        private static IEnumerator<WaitForSeconds> WaitThenDo(Action action)
        {
            yield return new WaitForSeconds(5f);
            action.Invoke();
        }

        private void ResetTimerStep()
        {
            var levelTimer = LevelTimer;
            levelTimer.Pause();
            levelTimer.ResetStep();
        }

        private void CreateTimerStep()
        {
            var levelTimer = LevelTimer;
            levelTimer.Pause();
            levelTimer.CreateStep();
        }

        private void SaveAttempt()
        {
            var levelTimer = LevelTimer;
            if (levelTimer == null)
                return;

            var steps = LevelSceneNames
                .Zip(
                    levelTimer.GetSteps(),
                    (level, time) => new LevelTime(level, time));
            var saveData = GameSaveData.Load();
            var attempts = saveData.attempts.ToList();
            attempts.Add(new Attempt(steps));
            saveData.attempts = attempts.ToArray();
            GameSaveData.Save(saveData);
        }

        private void Awake()
        {
            Instance ??= this;
            RemainingLives = Settings.InitialLivesCount;

            LoadLevel(LevelSceneNames[0]);
        }

        /// <summary>
        /// This method is called by a <see cref="PlayerInputManager"/> component, when a player was spawned.
        /// </summary>
        /// <param name="playerObject">the created game object for the new player</param>
        public void OnPlayerSpawned(GameObject playerObject)
        {
            if (playerObject == null)
                return;

            var tank = playerObject.GetComponent<Tank>();
            if (tank == null || tank.Eliminated == null)
                return;

            tank.Eliminated.AddListener(OnTankEliminated);

            var levelTimer = LevelTimer;
            if (levelTimer && levelTimer.Paused)
                levelTimer.Resume();
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
            loadSceneTask.completed += _ => MovePlayersToSpawn();
            loadSceneTask.completed += _ => FindObjectOfType<LevelTimer>()?.Resume();
            loadSceneTask.completed += _ => GameRulesActive = true;
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
                if (tank && tank.Eliminated != null)
                    tank.Eliminated.RemoveListener(OnTankEliminated);

            RemainingEnemyTanks = FindObjectsOfType<Tank>()!
                .Where(tank => tank && !tank.CompareTag("Player"))
                .ToArray();

            foreach (var tank in RemainingEnemyTanks)
                if (tank && tank.Eliminated != null)
                    tank.Eliminated.AddListener(OnTankEliminated);
        }

        private static void MovePlayersToSpawn()
        {
            var spawnArea = FindObjectOfType<SpawnArea>();
            if (spawnArea == null)
                return;

            spawnArea.ResetSpawns();
            var playerTanks = FindObjectsOfType<PlayerInput>();
            if (playerTanks == null)
                return;

            foreach (var playerTank in playerTanks)
            {
                if (playerTank == null)
                    continue;

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