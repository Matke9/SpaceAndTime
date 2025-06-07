using System;
using Unity.VisualScripting;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    //bool beingDragged = false;
    public Collider2D wallCollider;
    [SerializeField] private GameObject leftBrim;
    [SerializeField] private GameObject topBrim;
    [SerializeField] private GameObject rightBrim;
    [SerializeField] private GameObject bottomBrim;
    DragDropSystem dragDropSystem;

    private void Start()
    {
        dragDropSystem = FindFirstObjectByType<DragDropSystem>();
        wallCollider = GetComponent<CompositeCollider2D>();
        
        Vector3Int cellPos = dragDropSystem.grid.WorldToCell(transform.position);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        dragDropSystem.draggableObjects.Add(gridPos, gameObject);
        Vector3 startPos = dragDropSystem.grid.CellToWorld(new Vector3Int(cellPos.x,cellPos.y,0));
        startPos.x += 1f;
        startPos.y += 1f;
        startPos.z = 0;
        transform.position = startPos;
    }
    
    
    
}
