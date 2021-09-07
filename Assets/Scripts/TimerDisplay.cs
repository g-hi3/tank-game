using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private string timeFormat;
    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        _text.text = string.Format(
            timeFormat,
            timer.Milliseconds,
            timer.Seconds,
            timer.Minutes);
    }
}
