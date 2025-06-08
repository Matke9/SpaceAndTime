using UnityEngine;

public class DiodeParticleSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    private Enemy enemy;
    private bool played;
    void Start()
    {
        particles.Stop();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.isAttacking)
        {
            if (!played)
            {
                played = true;
                particles.Play();
            }
        }
        else
        {
            played=false;
            particles.Stop();
        }
    }
}
