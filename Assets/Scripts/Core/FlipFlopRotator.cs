using UnityEngine;

namespace TankGame.Core
{
    public class FlipFlopRotator : Rotator
    {
        private float _baseInput;
        private float _currentRotation;
        
        /// <summary>
        /// Rotation center in degrees, not radians; 90 is up
        /// </summary>
        [field: SerializeField]
        [field: Range(0f, 360f)]
        [field: Tooltip("Rotation center in degrees, not radians; 90 is up")]
        public float RotationCenter { get; private set; }

        /// <summary>
        /// Rotation range in degrees, not radians
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Rotation range in degrees, not radians")]
        public float RotationRange { get; private set; }

        /// <summary>
        /// Rotation speed; 0 stops, negative reverses
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Rotation speed; 0 stops, negative reverses")]
        public float RotationSpeed { get; private set; }
        
        /// <summary>
        /// Copies the rotation from the Transform component at start.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Copies the rotation from the Transform component at start.")]
        public bool UseSourceRotationAsCenter { get; private set; }

        public override void Rotate()
        {
            if (RotationRange == 0f)
                return;

            _currentRotation = GetRotationAngle();
            transform.rotation = Quaternion.RotateTowards(
                from: Quaternion.Euler(0f, 0f, transform.eulerAngles.z),
                to: Quaternion.Euler(0f, 0f, _currentRotation),
                maxDegreesDelta: 1f);
        }

        /// <summary>
        /// Calculates the flip flop rotation.
        /// </summary>
        /// <returns>rotation angle in degrees</returns>
        private float GetRotationAngle()
        {
            _baseInput += Time.deltaTime;
            var adjustedRotationRange = RotationRange * 0.5f;
            var adjustedRotationSpeed = RotationSpeed * 180f / RotationRange;
            return adjustedRotationRange * Mathf.Sin(adjustedRotationSpeed * _baseInput) + RotationCenter;
        }

        private void Start()
        {
            if (UseSourceRotationAsCenter)
                RotationCenter = transform.eulerAngles.z;
            else
                transform.eulerAngles = new Vector3(0f, 0f, RotationCenter);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, 0f, _currentRotation) * Vector3.right);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * Vector3.right);
        }
    }
}
