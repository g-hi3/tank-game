namespace TankGame.Core
{
    public readonly struct PlayerTankEliminatedEventArgs
    {
        public uint RemainingLives { get; }

        public PlayerTankEliminatedEventArgs(uint remainingLives)
        {
            RemainingLives = remainingLives;
        }
    }
}