using UnityEngine;

public class LevelTimer : Timer
{
    private int _totalMilliseconds;

    public override int Milliseconds => _totalMilliseconds % 1000;
    public override int Seconds => _totalMilliseconds / 1000 % 60;
    public override int Minutes => _totalMilliseconds / 60_000;

    private void Update()
    {
        _totalMilliseconds = (int)(Time.time * 1000);
    }
}
