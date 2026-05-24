using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] private float lerpSpeed = 15f;
    [SerializeField] private float rotationLerpSpeed = 5f;
    [SerializeField] private LayerMask collisionCheckMask; // Layers we test for collisions
    [SerializeField] private Vector2 tileCheckSize = new Vector2(1.8f, 1.8f); // Overlap-check box size
    
    private GameObject draggedObject;
    public Dictionary<Vector2Int, GameObject> draggableObjects = new Dictionary<Vector2Int, GameObject>();
    private bool isDragging = false;
    private bool isRotating = false;
    private Vector3 oldPosition;
    Quaternion targetRotation;
    private bool canPlace = true; // Whether the dragged object can be placed

    void Update()
    {
        bool paused = GameSystems.State?.IsPaused ?? false;
        if (Input.GetMouseButtonDown(0) && !paused)
        {
            if (draggableObjects.TryGetValue(GetMouseCell(), out draggedObject) && draggedObject.GetComponent<Draggable>().IsMovable)
            {
                // Check for collisions before starting the drag
                Vector3 targetPos = GetNewPositionInt();
                canPlace = !CheckTileOccupied(targetPos);
                if (canPlace)
                {
                    StartDragging(draggedObject);
                }
            }
        }
        
        if (isDragging)
        {
            draggedObject.transform.position = Vector3.Lerp(
                draggedObject.transform.position, 
                GetNewPositionMouse(), 
                lerpSpeed * Time.deltaTime
            );
            
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 currentRotation = draggedObject.transform.rotation.eulerAngles;
                targetRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z - 90f);
                isRotating = true;
                Vector3 brimRotation = draggedObject.GetComponent<Draggable>().outerBrim.transform.rotation.eulerAngles;
                brimRotation.z += 90f;
                draggedObject.GetComponent<Draggable>().outerBrim.transform.rotation = Quaternion.Euler(brimRotation);
            }

            // Rotate only while a rotation is in progress
            if (isRotating)
            {
                draggedObject.transform.rotation = Quaternion.RotateTowards(
                    draggedObject.transform.rotation,
                    targetRotation,
                    rotationLerpSpeed * 360f * Time.deltaTime // Multiply by 360 to get degrees per second
                );

                // Snap once we are close enough to the target rotation
                if (Quaternion.Angle(draggedObject.transform.rotation, targetRotation) < 0.1f)
                {
                    draggedObject.transform.rotation = targetRotation; // Snap exactly to target
                    isRotating = false;
                }
            }

        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }

    private bool CheckTileOccupied(Vector3 position)
    {
        // Check overlap with objects on the given layer mask
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            position,
            tileCheckSize,
            0f,
            collisionCheckMask
        );

        return colliders.Length > 0;
    }

    private void StartDragging(GameObject draggedObject)
    {
        Vector2Int oldPos = (Vector2Int)grid.WorldToCell(draggedObject.transform.position);
        draggableObjects.Remove(oldPos);
        UpdateTileAndNeighbors(oldPos);
        oldPosition = draggedObject.transform.position;
        isDragging = true;
        SetLayerRecursively(draggedObject, 8);
        targetRotation = draggedObject.transform.rotation;
        isRotating = false;
    }
    
    private void StopDragging()
    {
        Vector2Int newCell = GetMouseCell();
        Vector3Int oldCell = grid.WorldToCell(oldPosition);
        Vector2Int oldPos = new Vector2Int(oldCell.x, oldCell.y);
        
        if (!canPlace || draggableObjects.ContainsKey(newCell))
        {
            draggedObject.transform.position = oldPosition;
            draggableObjects.Add(oldPos, draggedObject);
            UpdateTileAndNeighbors(oldPos);
        }
        else
        {
            UpdateTileAndNeighbors(oldPos);
            draggedObject.transform.position = GetNewPositionInt();
            draggableObjects.Add(newCell, draggedObject);
            
            // Update neighbours at the new position
            UpdateTileAndNeighbors(newCell);
        }
        
        isDragging = false;
        SetLayerRecursively(draggedObject, 7);
    }

    public void UpdateTileAndNeighbors(Vector2Int centerPos)
    {
        // Update the centre tile
        UpdateOuterBrims(centerPos);
        
        // Update neighbours
        Vector2Int[] neighbors = new Vector2Int[]
        {
            centerPos + Vector2Int.right,
            centerPos + Vector2Int.left,
            centerPos + Vector2Int.up,
            centerPos + Vector2Int.down
        };

        foreach (Vector2Int neighbor in neighbors)
        {
            if (draggableObjects.ContainsKey(neighbor))
            {
                UpdateOuterBrims(neighbor);
            }
        }
    }

    Vector2Int GetMouseCell()
    {
        Vector3Int cellPos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    Vector3 GetNewPositionInt()
    {
        Vector2Int cellPos = GetMouseCell();
        Vector3 newPos = grid.CellToWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
        newPos.x += 1f;
        newPos.y += 1f;
        newPos.z = 0;
        return newPos;
    }

    Vector3 GetNewPositionMouse()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        return newPos;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void UpdateOuterBrims()
    {
        Vector2Int tilePos = (Vector2Int)grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        GameObject center_tile;
        bool has_center = draggableObjects.TryGetValue(tilePos, out center_tile);

        // Check all neighbouring tiles
        CheckAndUpdateWall(tilePos, Vector2Int.right); // Right
        CheckAndUpdateWall(tilePos, Vector2Int.left);  // Left
        CheckAndUpdateWall(tilePos, Vector2Int.up);    // Up
        CheckAndUpdateWall(tilePos, Vector2Int.down);  // Down
        
    }
    
    private void UpdateOuterBrims(Vector2Int tilePos)
    {
        GameObject currentTile;
        if (!draggableObjects.TryGetValue(tilePos, out currentTile)) return;

        // Check all neighbouring tiles
        CheckAndUpdateWall(tilePos, Vector2Int.right); // Right
        CheckAndUpdateWall(tilePos, Vector2Int.left);  // Left
        CheckAndUpdateWall(tilePos, Vector2Int.up);    // Up
        CheckAndUpdateWall(tilePos, Vector2Int.down);  // Down
    }


    private void CheckAndUpdateWall(Vector2Int centerPos, Vector2Int direction)
    {
        GameObject centerTile;
        if (draggableObjects.TryGetValue(centerPos, out centerTile))
        {
            Vector2Int neighborPos = centerPos + direction;
            bool hasNeighbor = draggableObjects.ContainsKey(neighborPos);

            // Get the wall object for this side
            Transform wall = GetWallForDirection(centerTile.transform, direction);
            if (wall != null)
            {
                // Enable the wall when there is no neighbour, disable it when there is
                wall.gameObject.SetActive(!hasNeighbor);
            }
        }
    }
    

    private Transform GetWallForDirection(Transform tileTransform, Vector2Int direction)
    {
        string wallName;
        if (direction == Vector2Int.right) wallName = "rightBrim";
        else if (direction == Vector2Int.left) wallName = "leftBrim";
        else if (direction == Vector2Int.up) wallName = "topBrim";
        else if (direction == Vector2Int.down) wallName = "bottomBrim";
        else return null;

        Transform outerBrim = tileTransform.Find("OuterBrim");
        if (outerBrim != null)
        {
            return outerBrim.Find(wallName);
        }
        return null;
    }

    // Called from Draggable.Start() to register a tile and refresh its neighbours
    public void InitializeTile(Vector2Int gridPos)
    {
        if (!draggableObjects.ContainsKey(gridPos))
        {
            GameObject tile = GameObject.Find(gridPos.ToString());
            draggableObjects.Add(gridPos, tile);
        }
        UpdateTileAndNeighbors(gridPos);
    }
}