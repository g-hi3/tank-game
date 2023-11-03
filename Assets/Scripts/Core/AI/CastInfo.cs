using System;
using System.Linq;
using UnityEngine;

namespace TankGame.Core.AI
{
    public class CastInfo
    {
        public static readonly CastInfo InvalidCast = CreateNoHit(directionCount: 0);

        public bool IsTargetHit { get; }
        public Vector3[] CastDirections { get; }
        public float TotalDistance => CastDirections.Sum(direction => direction.magnitude);

        private CastInfo(bool isTargetHit, int directionCount)
        {
            IsTargetHit = isTargetHit;
            CastDirections = directionCount > 0
                ? new Vector3[directionCount]
                : Array.Empty<Vector3>();
        }

        public static CastInfo CreateTargetHit(int directionCount)
        {
            return new CastInfo(isTargetHit: true, directionCount);
        }

        public static CastInfo CreateNoHit(int directionCount)
        {
            return new CastInfo(isTargetHit: false, directionCount);
        }
    }
}