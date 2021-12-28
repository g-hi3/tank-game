using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ConfettiController : MonoBehaviour
{
    private ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    private void OnGameOver()
    {
        _particles.Play();
    }
}
