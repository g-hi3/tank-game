using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TankGame.Core.AI;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public sealed class MultithreadingTankVision : MonoBehaviour, IDisposable, ITankVision
{
    public Transform TankHead;
    [Min(0f)] public float WaitTimeSeconds;
    [Min(0)] public int RayCount;
    [Range(0f, 360f)] public float ConeAngleDegrees;
    [Min(0f)] public float VisionDistance;
    [Min(0f)] public float CastRadius;
    [Min(0f)] public int MaxReflectionCount;
    public LayerMask TargetLayers;
    public LayerMask Reflectionlayers;
    private Transform _transform;
    private bool _continueCoroutine;
    private WaitForSeconds _waitTime;
    private DirectionJob _directionJob;
    private CustomRaycastHit[] _calculatedRaycastHits = Array.Empty<CustomRaycastHit>();
    private Multicast _optimalCast;
    
    private void Start()
    {
        _transform = GetComponent<Transform>();
        _continueCoroutine = true;
        _waitTime = new WaitForSeconds(WaitTimeSeconds);
        StartCoroutine(nameof(MulticastCoroutine));
    }

    private void OnDestroy()
    {
        _continueCoroutine = false;
        StopCoroutine(nameof(MulticastCoroutine));
        Dispose();
    }

    private IEnumerator MulticastCoroutine()
    {
        while (_continueCoroutine)
        {
            if (RayCount < 1)
            {
                Debug.LogWarning("RayCount is less than 1!", this);
                continue;
            }

            var actualRayCount = ConeAngleDegrees == 0f ? 1 : RayCount;
            using var directionJobResults = new NativeArray<Vector2>(actualRayCount, Allocator.Persistent);
            _directionJob.Directions = directionJobResults;
            _directionJob.Fidelity = (uint)actualRayCount;
            var actualBaseAngle = TankHead.eulerAngles.z - ConeAngleDegrees * 0.5f;
            _directionJob.BaseAngleDegrees = actualBaseAngle;
            _directionJob.ConeAngleDegrees = ConeAngleDegrees;
            var directionJobHandle = _directionJob.Schedule(actualRayCount, 1);
            yield return _waitTime;
            directionJobHandle.Complete();
            
            var actualMaxCastCount = MaxReflectionCount + 1;
            var calculatedRaycastHits = new List<CustomRaycastHit>();
            var allRaycastHits = new RaycastHit2D[2];
            var multicasts = new List<Multicast>();
            for (var i = 0; i < actualRayCount; i++)
            {
                var raycastHits = new CustomRaycastHit[actualMaxCastCount];
                for (var j = 0; j < actualMaxCastCount; j++)
                {
                    Vector2 origin = j == 0 ? _transform.position : raycastHits[j - 1].Centroid;
                    var direction = j == 0
                        ? _directionJob.Directions[i]
                        : Vector2.Reflect(raycastHits[j - 1].Direction, raycastHits[j - 1].Normal);
                    var distance = VisionDistance - (j == 0 ? 0 : raycastHits[j - 1].TotalDistance);
                    
                    if (distance <= 0f)
                        break;
                    
                    var hitCount = Physics2D.CircleCastNonAlloc(
                        origin,
                        CastRadius,
                        direction,
                        allRaycastHits,
                        distance,
                        layerMask: TargetLayers | Reflectionlayers);

                    if (hitCount == 0)
                        break;
                    
                    var raycastHit = allRaycastHits[0].distance > 0f
                        ? allRaycastHits[0]
                        : allRaycastHits[1];
                    raycastHits[j] = new CustomRaycastHit
                    {
                        Target = raycastHit.collider,
                        Origin = origin,
                        Direction = direction,
                        Centroid = raycastHit.centroid,
                        Normal = raycastHit.normal,
                        Distance = raycastHit.distance,
                        TotalDistance = (j == 0 ? 0 : raycastHits[j - 1].TotalDistance) + raycastHit.distance,
                        ReflectionCount = j
                    };
                    calculatedRaycastHits.Add(raycastHits[j]);
                    
                    if (raycastHits[j].Target == null)
                        break;

                    if (!IsLayerInMask(raycastHits[j].Target.gameObject.layer, TargetLayers))
                        continue;

                    multicasts.Add(new Multicast
                    {
                        IsHit = true,
                        StartDirection = raycastHits[0].Direction,
                        TotalDistance = raycastHits[j].TotalDistance,
                        ReflectionCount = raycastHits[j].ReflectionCount
                    });
                    break;
                }
            }

            _calculatedRaycastHits = calculatedRaycastHits.ToArray();
            _optimalCast = multicasts
                .OrderBy(cast => cast.TotalDistance)
                .ThenBy(cast => cast.ReflectionCount)
                .FirstOrDefault();
        }
    }

    private static bool IsLayerInMask(int layer, LayerMask mask) => (2 << layer & mask) != 0;

    private void OnDrawGizmos()
    {
        if (_optimalCast.IsHit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_transform.position, _optimalCast.StartDirection);
        }

        foreach (var raycastHit in _calculatedRaycastHits)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, (float)raycastHit.ReflectionCount / MaxReflectionCount);
            Gizmos.DrawRay(raycastHit.Origin, raycastHit.Direction * raycastHit.Distance);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(raycastHit.Centroid, raycastHit.Normal);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _directionJob.Dispose();
    }

    private struct DirectionJob : IJobParallelFor, IDisposable
    {
        public NativeArray<Vector2> Directions;
        [ReadOnly] public float Fidelity;
        [ReadOnly] public float BaseAngleDegrees;
        [ReadOnly] public float ConeAngleDegrees;

        /// <summary>
        /// Calculates the direction for multi casts.
        /// </summary>
        /// <param name="index">index of this job - also used for calculating the individual direction</param>
        public void Execute(int index)
        {
            var angleRadians = (BaseAngleDegrees + ConeAngleDegrees * index / Fidelity) * Mathf.Deg2Rad;
            var x = Mathf.Cos(angleRadians);
            var y = Mathf.Sin(angleRadians);
            Directions[index] = new Vector2(x, y);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Directions.Dispose();
        }
    }

    private struct CustomRaycastHit
    {
        [CanBeNull] public Collider2D Target;
        public Vector2 Origin;
        public Vector2 Direction;
        public Vector2 Centroid;
        public Vector2 Normal;
        public float Distance;
        public float TotalDistance;
        public int ReflectionCount;
    }

    private struct Multicast
    {
        public bool IsHit;
        public Vector2 StartDirection;
        public float TotalDistance;
        public int ReflectionCount;
    }

    /// <inheritdoc />
    public bool IsTargetVisible => _optimalCast.IsHit;

    /// <inheritdoc />
    public Vector3? GetBestTargetDirection()
    {
        return _optimalCast.StartDirection;
    }
}
