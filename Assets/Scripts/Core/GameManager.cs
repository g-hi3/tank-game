using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [field: SerializeField] public uint RemainingLives { get; private set; }
        [field: SerializeField] public Tank[] RemainingEnemyTanks { get; private set; }
        [field: SerializeField] public UnityEvent<PlayerTankEliminatedEventArgs> PlayerTankEliminated { get; private set; }
        [field: SerializeField] public UnityEvent AllEnemyTanksEliminated { get; private set; }
        [field: SerializeField] public UnityEvent GamePaused { get; private set; }
        [field: SerializeField] public UnityEvent GameResumed { get; private set; }

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
                PlayerTankEliminated?.Invoke(new PlayerTankEliminatedEventArgs(RemainingLives));
            else if (RemainingEnemyTanks.All(enemyTank => enemyTank == null || !enemyTank.CompareTag("Player") || enemyTank.gameObject == null))
                AllEnemyTanksEliminated?.Invoke();
        }

        private void Awake()
        {
            Instance ??= this;
        }

        private void Start()
        {
            RemainingEnemyTanks = FindObjectsOfType<Tank>()
                .Where(tank => !tank.CompareTag("Player"))
                .ToArray();

            foreach (var tank in RemainingEnemyTanks)
                tank.Eliminated.AddListener(OnTankEliminated);
        }
    }
}