using UnityEngine;

namespace TankGame.Core.AI
{
    public interface ITankVision
    {
        bool IsTargetVisible { get; }
        Vector3? GetBestTargetDirection();
    }
}