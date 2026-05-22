using System;
using UnityEngine;

public class GreenCollider : MonoBehaviour
{
    Green green;
    float cooldown = 0.1f;

    void Start()
    {
        green = transform.parent.GetComponent<Green>();
    }

    private void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Projectile projectile))
        {
            if (cooldown <= 0)
            {
                cooldown = 0.1f;
                projectile.Deactivate();
                if (!green.ChangePicture())
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

}

