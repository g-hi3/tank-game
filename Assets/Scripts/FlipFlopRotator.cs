using UnityEngine;

public class FlipFlopRotator : MonoBehaviour
{
    [SerializeField, Range(0f, 360f)] private float initialRotation;
    [SerializeField, Min(0f)] private float rotationRange;
    [SerializeField] private float rotationSpeed;
    private Transform _transform;
    private float _timeElapsed;

    private Quaternion GetCurrentRotation()
    {
        var rotationPercent = Mathf.PingPong(_timeElapsed * rotationSpeed, 1f) - 0.5f;
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

    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        _transform.rotation = GetCurrentRotation();
    }
}
