using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHud : MonoBehaviour
{
  [SerializeField] private Text currentLevelDisplay;
  [SerializeField] private Text speedrunTimerDisplay;
  private int _millisecondsAtStart;
  
  private void Start() {
    var activeScene = SceneManager.GetActiveScene();
    currentLevelDisplay.text = activeScene.name;
    _millisecondsAtStart = GetMillisecondsElapsed();
  }

  private void Update() {
    var millisecondsElapsed = GetMillisecondsElapsed() - _millisecondsAtStart;
    speedrunTimerDisplay.text = GetTimeString(millisecondsElapsed);
  }

  private static int GetMillisecondsElapsed() {
    return (int)(Time.time * 1000f);
  }

  private static string GetTimeString(int timeInMilliseconds) {
    var milliseconds = timeInMilliseconds % 1000;
    var seconds = timeInMilliseconds / 1000 % 60;
    var minutes = timeInMilliseconds / 60_000;
    return $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
  }
}
