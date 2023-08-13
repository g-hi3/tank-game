using UnityEngine;

namespace TankGame.Core
{
    public abstract class Rotator : MonoBehaviour, IRotator
    {
        public abstract void Rotate();
    }
}