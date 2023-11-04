using System;
using System.Linq;
using UnityEngine;

namespace TankGame.Core.AI
{
    /// <summary>
    /// Represents information about a reflection cast.
    /// </summary>
    public class CastInfo
    {
        /// <summary>
        /// Represents an invalid cast.
        /// </summary>
        /// <remarks>
        /// The value of this field is used to represent that a cast was not valid. Instances of this class can be
        /// compared to this to evaluate if a cast was valid.
        /// </remarks>
        public static readonly CastInfo InvalidCast = CreateNoHit(directionCount: 0);

        /// <summary>
        /// Describes whether the reflected ray cast hit a target.
        /// </summary>
        public bool IsTargetHit { get; }

        /// <summary>
        /// Contains all ray cast directions in order.
        /// </summary>
        public Vector3[] CastDirections { get; }

        /// <summary>
        /// Describes the total distance of all ray cast directions.
        /// </summary>
        public float TotalDistance => CastDirections.Sum(direction => direction.magnitude);

        private CastInfo(bool isTargetHit, int directionCount)
        {
            IsTargetHit = isTargetHit;
            CastDirections = directionCount > 0
                ? new Vector3[directionCount]
                : Array.Empty<Vector3>();
        }

        /// <summary>
        /// Creates a new instance for a ray cast that hit its target.
        /// </summary>
        /// <param name="directionCount">number of directions of the reflected ray cast</param>
        /// <returns>a new instance where <see cref="IsTargetHit"/> is <c>true</c></returns>
        public static CastInfo CreateTargetHit(int directionCount)
        {
            return new CastInfo(isTargetHit: true, directionCount);
        }

        /// <summary>
        /// Creates a new instance for a ray cast that didn't hit its target.
        /// </summary>
        /// <param name="directionCount">number of directions of the reflected ray cast</param>
        /// <returns>a new instance where <see cref="IsTargetHit"/> is <c>false</c></returns>
        public static CastInfo CreateNoHit(int directionCount)
        {
            return new CastInfo(isTargetHit: false, directionCount);
        }
    }
}