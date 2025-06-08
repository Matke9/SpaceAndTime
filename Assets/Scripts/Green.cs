using UnityEngine;

public class Green : MonoBehaviour
{
    [SerializeField] private Sprite[] stages;
    [SerializeField] private int stage;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = stages[stage];
    }
    
    public bool changePicture()
    {
        if (stage + 1 < stages.Length)
        {
            stage++;
            spriteRenderer.sprite = stages[stage];
            return stage != stages.Length - 1;
        }
        return false;
    }
}
