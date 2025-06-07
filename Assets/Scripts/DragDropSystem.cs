using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] private float lerpSpeed = 15f; // Dodajemo kontrolu brzine lerpa
    private GameObject draggedObject;

    public Dictionary<Vector2Int, GameObject> draggableObjects =
        new Dictionary<Vector2Int, GameObject>();
    bool is_dragging = false;
    private Vector3 oldPosition;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
            // Koristimo Time.deltaTime za smooth lerp
            draggedObject.transform.position = Vector3.Lerp(
                draggedObject.transform.position, 
                GetNewPositionMouse(), 
                lerpSpeed * Time.deltaTime
            );

            if (Input.GetMouseButtonDown(1)) // Promenili smo u MouseButtonDown da ne rotira kontinualno
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
        SetLayerRecursively(draggedObject, 8); // Postavljamo layer rekurzivno
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
        SetLayerRecursively(draggedObject, 7); // Vraćamo layer rekurzivno
    }

    // Nova metoda koja rekurzivno postavlja layer za objekat i svu njegovu decu
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        
        obj.layer = newLayer;
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}