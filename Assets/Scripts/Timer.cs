using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public abstract int Milliseconds { get; }
    public abstract int Seconds { get; }
    public abstract int Minutes { get; }
}