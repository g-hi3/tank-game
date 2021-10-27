using UnityEngine;

public class FlipFlopRotator : MonoBehaviour
{
    [SerializeField, Range(0f, 360f)] private float initialRotation;
    [SerializeField, Min(0f)] private float rotationRange;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float adjustmentSpeed;
    private Transform _transform;

    private Quaternion GetExpectedRotation(float time)
    {
        var rotationPercent = Mathf.PingPong(time * rotationSpeed, 1f) - 0.5f;
        var currentRotation = initialRotation + rotationRange * rotationPercent;
        return Quaternion.Euler(0f, 0f, currentRotation);
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _transform.rotation = Quaternion.Euler(0f, 0f, initialRotation);
    }

    private void FixedUpdate()
    {
        var expectedRotation = GetExpectedRotation(Time.fixedTime);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, expectedRotation, adjustmentSpeed);
    }
}
