using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] private float lerpSpeed = 15f;
    [SerializeField] private float rotationLerpSpeed = 5f;
    [SerializeField] private LayerMask collisionCheckMask; // Layeri koje proveravamo
    [SerializeField] private Vector2 tileCheckSize = new Vector2(2f, 2f); // Veličina provere
    
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
        oldPosition = draggedObject.transform.position;
        is_dragging = true;
        SetLayerRecursively(draggedObject, 8);
        targetRotation = draggedObject.transform.rotation;
        is_rotating = false;

    }
    
    private void StopDragging()
    {
        Vector2Int newCell = GetMouseCell();
        
        // Proveravamo da li možemo postaviti objekat na novu poziciju
        if (!canPlace || draggableObjects.ContainsKey(newCell))
        {
            // Ako ne možemo, vraćamo na staru poziciju
            draggedObject.transform.position = oldPosition;
        }
        else
        {
            // Ako možemo, postavljamo na novu poziciju
            Vector3Int oldCell = grid.WorldToCell(oldPosition);
            draggableObjects.Remove(new Vector2Int(oldCell.x, oldCell.y));
            draggedObject.transform.position = GetNewPositionInt();
            draggableObjects.Add(newCell, draggedObject);
        }

        // Resetujemo vizuelni feedback
        SpriteRenderer sprite = draggedObject.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color color = sprite.color;
            color.a = 1f;
            sprite.color = color;
        }

        is_dragging = false;
        SetLayerRecursively(draggedObject, 7);
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
        newPos.x += 0.5f;
        newPos.y += 0.5f;
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

    // Vizuelni prikaz područja provere u editoru
    private void OnDrawGizmos()
    {
        if (is_dragging)
        {
            Gizmos.color = canPlace ? Color.green : Color.red;
            Vector3 position = GetNewPositionInt();
            Gizmos.DrawWireCube(position, new Vector3(tileCheckSize.x, tileCheckSize.y, 0.1f));
        }
    }
}