using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] public Grid grid;
    private GameObject draggedObject;

    public Dictionary<Vector2Int, GameObject> draggableObjects =
        new Dictionary<Vector2Int, GameObject>();
    bool is_dragging = false;
    private Vector3 oldPosition;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            if (draggableObjects.TryGetValue(GetMouseCell(), out draggedObject))
            {
                StartDragging(draggedObject);
                Debug.Log(draggedObject);
            }
        }
        if (Input.GetMouseButtonUp(0) && is_dragging)
        {
            StopDragging();
        }

        if (is_dragging)
        {
            draggedObject.transform.position = GetNewPositionMouse();
            if (Input.GetMouseButtonUp(1))
            {
                Vector3 rotation = draggedObject.transform.rotation.eulerAngles;
                rotation.z -= 90;
                draggedObject.transform.rotation = Quaternion.Euler(rotation);
            }
        }
    }

    Vector2Int GetMouseCell()
    {
        Vector3Int cellPos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);
        return gridPos;
    }

    Vector3 GetNewPositionInt()
    {
        Vector2Int cellPos = GetMouseCell();
        Vector3 newPos = grid.CellToWorld(new Vector3Int(cellPos.x,cellPos.y,0));
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
    
    private void StartDragging(GameObject draggedObject)
    {
        oldPosition = draggedObject.transform.position;
        is_dragging = true;
        Draggable draggable = draggedObject.GetComponent<Draggable>();
        draggable.collider.enabled = false;
    }
    
    private void StopDragging()
    {
        GameObject oldObject;
        if (draggableObjects.TryGetValue(GetMouseCell(), out oldObject))
        {
            draggedObject.transform.position = oldPosition;
        }
        else
        {
            Vector3Int oldCell = grid.WorldToCell(oldPosition);
            draggableObjects.Remove(new Vector2Int(oldCell.x, oldCell.y));
            Vector2Int cellPos = GetMouseCell();
            draggedObject.transform.position = GetNewPositionInt(); 
            draggableObjects.Add(cellPos, draggedObject);
        }
        is_dragging = false;
    }

    void UpdateDraggedLayer()
    {
        if (is_dragging)
        {
            draggedObject.GetComponent<CompositeCollider2D>().enabled = false;
        }
    }
}
