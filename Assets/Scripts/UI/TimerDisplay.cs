using TankGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private LevelTimer timer;
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
        }
    }
}
