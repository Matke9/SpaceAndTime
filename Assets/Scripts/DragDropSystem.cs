using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] private float lerpSpeed = 15f;
    [SerializeField] private float rotationLerpSpeed = 5f;
    [SerializeField] private LayerMask collisionCheckMask; // Layeri koje proveravamo
    [SerializeField] private Vector2 tileCheckSize = new Vector2(1.8f, 1.8f); // Veličina provere
    
    private GameObject draggedObject;
    public Dictionary<Vector2Int, GameObject> draggableObjects = new Dictionary<Vector2Int, GameObject>();
    private bool is_dragging = false;
    private bool is_rotating = false;
    private Vector3 oldPosition;
    Quaternion targetRotation;
    private bool canPlace = true; // Flag za proveru da li možemo postaviti objekat

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (draggableObjects.TryGetValue(GetMouseCell(), out draggedObject))
            {
                // Proveravamo kolizije tokom prevlačenja
                Vector3 targetPos = GetNewPositionInt();
                canPlace = !CheckTileOccupied(targetPos);
                if (canPlace)
                {
                    StartDragging(draggedObject);
                }
            }
        }
        
        if (is_dragging)
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
                is_rotating = true;
                Vector3 brimRotation = draggedObject.GetComponent<Draggable>().outerBrim.transform.rotation.eulerAngles;
                brimRotation.z += 90f;
                draggedObject.GetComponent<Draggable>().outerBrim.transform.rotation = Quaternion.Euler(brimRotation);
            }

            // Rotacija samo kad je potrebna
            if (is_rotating)
            {
                draggedObject.transform.rotation = Quaternion.RotateTowards(
                    draggedObject.transform.rotation,
                    targetRotation,
                    rotationLerpSpeed * 360f * Time.deltaTime // Množimo sa 360 da dobijemo stepene po sekundi
                );

                // Provera da li smo blizu ciljne rotacije
                if (Quaternion.Angle(draggedObject.transform.rotation, targetRotation) < 0.1f)
                {
                    draggedObject.transform.rotation = targetRotation; // Postavljamo tačno na cilj
                    is_rotating = false;
                }
            }

        }

        if (Input.GetMouseButtonUp(0) && is_dragging)
        {
            StopDragging();
        }
    }

    private bool CheckTileOccupied(Vector3 position)
    {
        // Proveravamo overlap sa objektima iz specifičnog layer mask-a
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
        is_dragging = true;
        SetLayerRecursively(draggedObject, 8);
        targetRotation = draggedObject.transform.rotation;
        is_rotating = false;
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
            
            // Update-ujemo susede na novoj poziciji
            UpdateTileAndNeighbors(newCell);
        }
        
        is_dragging = false;
        SetLayerRecursively(draggedObject, 7);
    }

    public void UpdateTileAndNeighbors(Vector2Int centerPos)
    {
        // Update centralnog tile-a
        UpdateOuterBrims(centerPos);
        
        // Update suseda
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

        // Proveravamo sve susedne tile-ove
        CheckAndUpdateWall(tilePos, Vector2Int.right); // Desno
        CheckAndUpdateWall(tilePos, Vector2Int.left);  // Levo
        CheckAndUpdateWall(tilePos, Vector2Int.up);    // Gore
        CheckAndUpdateWall(tilePos, Vector2Int.down);  // Dole
        
    }
    
    private void UpdateOuterBrims(Vector2Int tilePos)
    {
        GameObject currentTile;
        if (!draggableObjects.TryGetValue(tilePos, out currentTile)) return;

        // Proveravamo sve susedne tile-ove
        CheckAndUpdateWall(tilePos, Vector2Int.right); // Desno
        CheckAndUpdateWall(tilePos, Vector2Int.left);  // Levo
        CheckAndUpdateWall(tilePos, Vector2Int.up);    // Gore
        CheckAndUpdateWall(tilePos, Vector2Int.down);  // Dole
    }


    private void CheckAndUpdateWall(Vector2Int centerPos, Vector2Int direction)
    {
        GameObject centerTile;
        if (draggableObjects.TryGetValue(centerPos, out centerTile))
        {
            Vector2Int neighborPos = centerPos + direction;
            bool hasNeighbor = draggableObjects.ContainsKey(neighborPos);

            // Dobavljamo wall objekat za trenutnu stranu
            Transform wall = GetWallForDirection(centerTile.transform, direction);
            if (wall != null)
            {
                // Aktiviramo zid ako nema suseda, deaktiviramo ako ima
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

    // Modifikujemo Draggable.Start() da inicijalizuje sve susede
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