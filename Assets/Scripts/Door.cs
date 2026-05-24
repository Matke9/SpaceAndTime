using UnityEngine;

public class Door : MonoBehaviour
{
    
    private PlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null && playerController.UseKey())
        {
            GameSystems.State?.TriggerLevelCleared();
        }
    }
}
