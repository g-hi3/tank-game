using UnityEngine;

public class BombController : MonoBehaviour {

  private static readonly int TriggerNameExplosionTrigger = Animator.StringToHash("Explosion Trigger");
  [SerializeField] private float lifetimeSeconds;
  [SerializeField] private Vector3 explosionScale;
  private Transform _transform;
  private Animator _animator;
  private float _remainingLifetimeSeconds;
  private bool _explosionActive;

  private void Explode() {
    _transform.localScale = explosionScale; 
    _animator.SetTrigger(TriggerNameExplosionTrigger);
    _explosionActive = true;
  }

  private void OnExplosionEnded()
  {
    Destroy(gameObject);
  }

  private void Awake()
  {
    _transform = GetComponent<Transform>();
    _animator = GetComponent<Animator>();
  }

  private void Start() {
    _remainingLifetimeSeconds = lifetimeSeconds;
  }

  private void Update() {
    _remainingLifetimeSeconds -= Time.deltaTime;
    if (_remainingLifetimeSeconds <= 0f) {
      Explode();
    }
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.HasComponent<BulletController>()) {
      Explode();
      return;
    }

    if (!_explosionActive
        || !other.gameObject.HasComponent<TankController>()) {
      return;
    }
    
    var tankController = other.GetComponent<TankController>();
    tankController.Die();
  }
}