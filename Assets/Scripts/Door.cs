using UnityEngine;

public class Door : MonoBehaviour
{
    
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null && playerController.useKey())
        {
            GameTimeManager.ReduceTime(1000);
            Debug.Log("CRAZYYY");
        }
    }
}
