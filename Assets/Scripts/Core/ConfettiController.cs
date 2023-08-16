using UnityEngine;

namespace TankGame.Core
{
    public class ConfettiController : MonoBehaviour
    {
        private ParticleSystem _particles;

        public void OnAllEnemyTanksEliminated()
        {
            _particles.Play();
        }

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
        }
    }
}
