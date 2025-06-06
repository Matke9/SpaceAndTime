using System;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    bool beingDragged = false;
    public Collider2D collider;

    private void Start()
    {
        collider = GetComponent<CompositeCollider2D>();
    }
    
    
}
