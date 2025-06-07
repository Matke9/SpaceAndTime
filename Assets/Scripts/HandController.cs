using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private GameObject handIdle;
    [SerializeField] private GameObject handPinch;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handIdle.SetActive(true);
        handPinch.SetActive(false);
    }

    // Update is called once per frame
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
