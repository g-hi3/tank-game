namespace TankGame.Core.Bomb
{
    /// <summary>
    /// Implementing types react when making contact with a explosion.
    /// </summary>
    public interface IDetonationTarget
    {
        /// <summary>
        /// This method is executed when the game object made contact with an explosion.
        /// </summary>
        void OnDetonationHit();
    }
}