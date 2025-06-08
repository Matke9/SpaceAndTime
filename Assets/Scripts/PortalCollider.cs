using System;
using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    private Portal portal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portal = transform.parent.GetChild(0).GetComponent<Portal>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() != null && portal.finished == false)
        {
            portal.changePicture();
        }
        if (other.GetComponent<PlayerController>() != null && portal.finished == true)
        {
            Debug.Log("CRAZYYY");
            portal.teleport();
        }
    }
}
