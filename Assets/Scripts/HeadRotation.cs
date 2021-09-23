using System;
using UnityEngine;

[Serializable]
public class HeadRotation
{
    [SerializeField, Range(0f, 360f)] private float initialRotation;
    [SerializeField, Min(0f)] private float rotationRange;
    [SerializeField] private float rotationSpeed;

    public Quaternion InitialRotation => Quaternion.Euler(0f, 0f, initialRotation);
    public Quaternion CurrentRotation
    {
        get
        {
            var headRotationPercent = Mathf.PingPong(Time.time * rotationSpeed, 1f) - 0.5f;
            var currentHeadRotation = initialRotation + rotationRange * headRotationPercent;
            return Quaternion.Euler(0f, 0f, currentHeadRotation);
        }
    }
}
