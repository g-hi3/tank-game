namespace TankGame.Core.AI
{
    /// <summary>
    /// This component is specialized for rotating towards a target.
    /// </summary>
    public abstract class TargetingRotator : Rotator
    {
        /// <summary>
        /// Determines whether a target can be rotated towards.
        /// </summary>
        /// <returns><c>true</c> if a target can be rotated towards; otherwise <c>false</c></returns>
        public abstract bool IsTargetInSight();
    }
}