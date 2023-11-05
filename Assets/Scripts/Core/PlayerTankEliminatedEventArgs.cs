namespace TankGame.Core
{
    /// <summary>
    /// Contains relevant data for the player tank eliminated event.
    /// </summary>
    public readonly struct PlayerTankEliminatedEventArgs
    {
        /// <summary>
        /// Represents how many lives the player has <i>after</i> this elimination.
        /// </summary>
        public uint RemainingLives { get; }

        /// <summary>
        /// Creates a new instance using the given <paramref name="remainingLives"/>.
        /// </summary>
        /// <param name="remainingLives">number of lives the player has after this elimination</param>
        public PlayerTankEliminatedEventArgs(uint remainingLives)
        {
            RemainingLives = remainingLives;
        }
    }
}