using UnityEngine;

public class DiodeParticleSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    private Enemy enemy;
    void Start()
    {
        particleSystem.Stop();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        Debug.Log("a");
        if (enemy.canSeePlayer && enemy.hasReachedLastPosition)
        {
            Debug.Log("b");
            particleSystem.Play();
        }
        else
        {
            particleSystem.Stop();
        }
    }
}
