using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankController : MonoBehaviour {

  private const float RotationZOffset = 270f;
  private static readonly int Property = Animator.StringToHash("Move Speed");
  [SerializeField] private Transform head;
  [SerializeField] private float moveSpeed;
  [SerializeField] private GameObject bombTemplate;
  [SerializeField] private int shootCapacity;
  [SerializeField] private int bombCapacity;
  [SerializeField] private GameObject bulletTemplate;
  [SerializeField] private Transform bulletSpawn;
  private Transform _transform;
  private Rigidbody2D _rigidbody;
  private Animator _animator;
  private bool _isMoving;
  private Vector3 _moveDirection;
  private Quaternion _moveRotation;
  private Quaternion _lookRotation;
  private ICollection<GameObject> _plantedBombs = new List<GameObject>();
  private ICollection<GameObject> _firedShots = new List<GameObject>();

  public void Die()
  {
    Destroy(gameObject);
  }
  
  private void StartMoving() {
    _isMoving = true;
    _animator.SetFloat(Property, 1f);
  }
  
  private void Move(Vector3 direction) {
    _moveDirection = moveSpeed * direction;
    _moveRotation = GetRotation2DInDirection(direction);
  }

  private static Quaternion GetRotation2DInDirection(Vector3 direction) {
    var rotationZ = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - RotationZOffset;
    return Quaternion.Euler(0f, 0f, rotationZ);
  }

  private void StopMoving() {
    _isMoving = false;
    _animator.SetFloat(Property, 0f);
  }

  private void Look(Vector2 direction) {
    _lookRotation = GetRotation2DInDirection(direction);
  }

  private void LookAt(Vector3 position) {
    var direction = position - _transform.position;
    Look(direction);
  }

  private void Shoot() {
    _firedShots = _firedShots.Where(g => g != null).ToList();
    if (_firedShots.Count >= shootCapacity) {
      return;
    }
    var bullet = Instantiate(bulletTemplate, bulletSpawn.position, _lookRotation);
    bullet.layer = gameObject.layer;
    _firedShots.Add(bullet);
  }

  private void Bomb() {
    _plantedBombs = _plantedBombs.Where(g => g != null).ToList();
    if (_plantedBombs.Count >= bombCapacity) {
      return;
    }
    var plantedBomb = Instantiate(bombTemplate, _transform.position, _transform.rotation);
    _plantedBombs.Add(plantedBomb);
  }

  private void Awake() {
    _transform = GetComponent<Transform>();
    _rigidbody = GetComponent<Rigidbody2D>();
    _animator = GetComponentInChildren<Animator>();
  }

  private void Start() {
    _lookRotation = _transform.rotation;
  }

  private void Update() {
    _transform.rotation = _moveRotation;
    head.rotation = _lookRotation;
  }

  private void FixedUpdate() {
    if (_isMoving) {
      _rigidbody.velocity = _moveDirection;
    }
  }
}