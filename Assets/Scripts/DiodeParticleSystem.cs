using UnityEngine;

public class DiodeParticleSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    private Enemy enemy;
    private bool played;
    void Start()
    {
        particleSystem.Stop();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.isAttacking)
        {
            if (!played)
            {


                played = true;
                particleSystem.Play();
                Debug.Log("played");
            }
        }
        else
        {
            played=false;
            particleSystem.Stop();
        }
    }
}
