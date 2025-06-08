using System;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType{
    YELLOW, RED, BLUE, GREEN, PURPLE, ORANGE, BLACK, WHITE, GRAY, BROWN, PINK, BROWN_DARK, PINK_DARK, YELLOW_DARK, RED_DARK, BLUE_DARK, GREEN_DARK, PURPLE_DARK, ORANGE_DARK, BLACK_DARK, WHITE_DARK, GRAY_DARK
}

public class Draggable : MonoBehaviour
{
    //bool beingDragged = false;
    public Collider2D wallCollider;
    [SerializeField] public TileType tileType;
    [SerializeField] public GameObject outerBrim;
    DragDropSystem dragDropSystem;

    private void Start()
    {
        dragDropSystem = FindFirstObjectByType<DragDropSystem>();
        wallCollider = GetComponent<CompositeCollider2D>();
        
        Vector3Int cellPos = dragDropSystem.grid.WorldToCell(transform.position);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        dragDropSystem.draggableObjects.Add(gridPos, gameObject);
        
        Vector3 startPos = dragDropSystem.grid.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        startPos.x += 1f;
        startPos.y += 1f;
        startPos.z = 0;
        transform.position = startPos;
        
        dragDropSystem.UpdateTileAndNeighbors(gridPos);
        
        outerBrim.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z);
    }
}