using UnityEngine;

public class FrameRateRegulator : MonoBehaviour {
  [SerializeField] [Min(1)] private int _targetFrameRate;
  
  private void Start() {
    Application.targetFrameRate = _targetFrameRate;
  }
}
