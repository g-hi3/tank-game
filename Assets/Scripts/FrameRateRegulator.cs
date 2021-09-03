using UnityEngine;

public class FrameRateRegulator : MonoBehaviour
{
    [SerializeField] [Min(1)] private int targetFrameRate;
  
    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}