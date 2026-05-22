using System;
using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    private Portal portal;

    void Start()
    {
        portal = transform.parent.GetChild(0).GetComponent<Portal>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() != null && portal.finished == false)
        {
            portal.ChangePicture();
        }
        if (other.GetComponent<PlayerController>() != null && portal.finished == true)
        {
            portal.Teleport();
        }
    }
}
