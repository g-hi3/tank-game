using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankController : MonoBehaviour {

  private const float RotationZOffset = 270f;
  private static readonly int Property = Animator.StringToHash("Move Speed");
  [SerializeField] private Transform head;
  [SerializeField] private float moveSpeed = 1f;
  [SerializeField] private GameObject bombTemplate;
  [SerializeField] private int shootCapacity = 4;
  [SerializeField] private int bombCapacity = 4;
  [SerializeField] private GameObject bulletTemplate;
  [SerializeField] private Transform bulletSpawn;
  private ITankInput _tankInput;
  private Transform _transform;
  private Rigidbody2D _rigidbody;
  private Animator _animator;
  private float _moveSpeed;
  private Vector3 _moveDirection;
  private Quaternion _moveRotation;
  private Quaternion _lookRotation;
  private ICollection<GameObject> _plantedBombs = new List<GameObject>();
  private ICollection<GameObject> _firedShots = new List<GameObject>();

  private void StartMoving() {
    _moveSpeed = moveSpeed;
    _animator.SetFloat(Property, 1f);
  }
  
  private void Move(Vector3 direction) {
    _moveDirection = _moveSpeed * direction;
    var rotationZ = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - RotationZOffset;
    _moveRotation = Quaternion.Euler(0f, 0f, rotationZ);
  }

  private void StopMoving() {
    _moveSpeed = 0f;
    _animator.SetFloat(Property, 0f);
  }

  private void Look(Vector2 direction) {
    var rotationZ = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - RotationZOffset;
    _lookRotation = Quaternion.Euler(0f, 0f, rotationZ);
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

  private void TankMovingStarted(object sender, TankMovingStartedEventArgs eventArgs) {
    StartMoving();
  }

  private void TankMoved(object sender, TankMovedEventArgs eventArgs) {
    Move(eventArgs.MoveDirection);
  }

  private void TankMovingCanceled(object sender, TankMovingCanceledEventArgs eventArgs) {
    StopMoving();
  }

  private void TankLooked(object sender, TankLookedEventArgs eventArgs) {
    Look(eventArgs.LookDirection);
  }

  private void TankLookedAt(object sender, TankLookedAtEventArgs eventArgs) {
    LookAt(eventArgs.LookPosition);
  }

  private void TankShot(object sender, TankShotEventArgs eventArgs) {
    Shoot();
  }

  private void TankBombed(object sender, TankBombedEventArgs eventArgs) {
    Bomb();
  }

  private void Awake() {
    _tankInput = GetComponent<ITankInput>();
    _transform = GetComponent<Transform>();
    _rigidbody = GetComponent<Rigidbody2D>();
    _animator = GetComponentInChildren<Animator>();
  }

  private void Start() {
    _tankInput.TankMovingStarted += TankMovingStarted;
    _tankInput.TankMoved += TankMoved;
    _tankInput.TankMovingCanceled += TankMovingCanceled;
    _tankInput.TankLooked += TankLooked;
    _tankInput.TankLookedAt += TankLookedAt;
    _tankInput.TankShot += TankShot;
    _tankInput.TankBombed += TankBombed;
    _lookRotation = _transform.rotation;
  }

  private void Update() {
    _transform.rotation = _moveRotation;
    head.rotation = _lookRotation;
  }

  private void FixedUpdate() {
    if (_moveSpeed > float.Epsilon) {
      _rigidbody.MovePosition(_transform.position + Time.fixedDeltaTime * _moveDirection);
    }
  }

  private void OnDestroy() {
    _tankInput.TankMoved -= TankMoved;
    _tankInput.TankLooked -= TankLooked;
    _tankInput.TankShot -= TankShot;
    _tankInput.TankBombed -= TankBombed;
  }
}