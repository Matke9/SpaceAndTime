using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private GameObject handIdle;
    [SerializeField] private GameObject handPinch;

    void Start()
    {
        handIdle.SetActive(true);
        handPinch.SetActive(false);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            handIdle.SetActive(false);
            handPinch.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            handIdle.SetActive(true);
            handPinch.SetActive(false);
        }
    }
}
