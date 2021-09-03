using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int ricochetCount;
    private Transform _transform;
    private int _remainingRicochets;

    private void ReflectFrom(ContactPoint2D contactPoint)
    {
        var reflected = Vector2.Reflect(_transform.right, contactPoint.normal);
        var rotationZ = 90f - Mathf.Atan2(reflected.x, reflected.y) * Mathf.Rad2Deg;
        _transform.eulerAngles = new Vector3(0f, 0f, rotationZ);
        _remainingRicochets--;
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _remainingRicochets = ricochetCount;
    }

    private void Update()
    {
        if (_remainingRicochets < 1)
        {
            Destroy(gameObject);
        }
        _transform.Translate(Time.deltaTime * moveSpeed * Vector3.right);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.HasComponent<BulletController>())
        {
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.TryGetComponent<TankController>(out var tankController))
        {
            tankController.Die();
            Destroy(gameObject);
            return;
        }
    
        if (gameObject.layer == other.gameObject.layer)
        {
            Physics2D.IgnoreCollision(other.collider, other.otherCollider);
            return;
        }

        ReflectFrom(other.contacts[0]);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.HasComponent<BombController>())
        {
            Destroy(gameObject);
        }
    }
}
