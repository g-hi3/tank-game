using UnityEngine;

public class BombController : MonoBehaviour {

  [SerializeField] private float lifetimeSeconds = 5f;
  private float _remainingLifetimeSeconds;

  private void Explode() {
    Destroy(gameObject);
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
    }
  }
}