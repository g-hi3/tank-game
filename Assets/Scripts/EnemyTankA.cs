using UnityEngine;

public class EnemyTankA : MonoBehaviour
{
    [SerializeField] private HeadRotation headRotation;
    [SerializeField] private GameObject head;
    private Transform _headTransform;

    private void Awake()
    {
        _headTransform = head.GetComponent<Transform>();
    }

    private void Start()
    {
        _headTransform.rotation = headRotation.InitialRotation;
    }

    private void Update()
    {
        _headTransform.rotation = headRotation.CurrentRotation;
    }
}
