using UnityEngine;

public class DragDropSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;
    private GameObject draggedObject;
    bool is_dragging = false;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(grid.WorldToCell(Input.mousePosition));
        }
    }

    void UpdateDraggedLayer()
    {
        if (is_dragging)
        {
            draggedObject.GetComponent<CompositeCollider2D>().enabled = false;
        }
    }
}
