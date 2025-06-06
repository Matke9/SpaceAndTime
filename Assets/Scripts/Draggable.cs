using System;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    bool beingDragged = false;
    public Collider2D collider;
    DragDropSystem dragDropSystem;

    private void Start()
    {
        dragDropSystem = FindFirstObjectByType<DragDropSystem>();
        collider = GetComponent<CompositeCollider2D>();
        
        Vector3Int cellPos = dragDropSystem.grid.WorldToCell(transform.position);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        dragDropSystem.draggableObjects.Add(gridPos, gameObject);

    }
    
    
}
