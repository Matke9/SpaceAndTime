using UnityEngine;

public class GreenCollider : MonoBehaviour
{
    Green green;
    void Start()
    {
        green = transform.parent.GetComponent<Green>();
    }

    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() != null)
        {
            if (!green.changePicture())
            {
                gameObject.SetActive(false);
            }
        }
        
    }
}
