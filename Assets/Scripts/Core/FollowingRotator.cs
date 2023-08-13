using UnityEngine;

namespace TankGame.Core
{
    public class FollowingRotator : MonoBehaviour, IRotator
    {
        private TankVision _tankVision;
        private int _temp;
        
        [field: SerializeField] public float RotationSpeed { get; private set; }

        /// <summary>
        /// Rotator to be used when no target is in sight
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Rotator to be used when no target is in sight")]
        public Rotator FallbackRotator { get; private set; }

        public void Rotate()
        {
            if (_temp % 100 > 0)
                FallbackRotator.Rotate();
            else
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            _temp++;
            /*Vector2 currentDirection = transform.eulerAngles;
            Vector2 targetDirection = _tankVision.GetBestTargetDirection().normalized;

            if (Vector2.Distance(currentDirection, targetDirection) >= 0.02f)
                transform.eulerAngles = Vector2.MoveTowards(currentDirection, targetDirection, RotationSpeed);*/
        }

        private void Awake()
        {
            _tankVision = GetComponent<TankVision>();
        }

        private void Update()
        {
            Rotate();
        }
    }
}
