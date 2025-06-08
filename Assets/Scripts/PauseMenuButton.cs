using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Transform leftCursor;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        leftCursor.transform.position = transform.position;
    }
}
