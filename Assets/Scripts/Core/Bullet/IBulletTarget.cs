namespace TankGame.Core.Bullet
{
    /// <summary>
    /// Components that implement this interface will react if a bullet hits their game object.
    /// </summary>
    public interface IBulletTarget
    {
        /// <summary>
        /// This method is called when the object collides with a bullet.
        /// </summary>
        void OnBulletHit();
    }
}