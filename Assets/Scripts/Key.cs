using UnityEngine;

public class Key : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerController.PickUpKey();
            Destroy(gameObject);
        }
    }
}
