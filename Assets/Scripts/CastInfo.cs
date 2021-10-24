using System;
using System.Linq;
using UnityEngine;

public class CastInfo
{
    public static readonly CastInfo InvalidCast = new CastInfo();

    public bool IsTargetHit { get; set; }
    public Vector3[] CastDirections { get; set; } = Array.Empty<Vector3>();
    public float TotalDistance => CastDirections.Sum(direction => direction.magnitude);
}