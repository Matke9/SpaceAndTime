using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Sprite[] stages;
    [SerializeField] private int stage;
    [SerializeField] public bool finished;
    [SerializeField] private Transform destination;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;
    void Start()
    {
        if (!finished)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = stages[stage];
            animator = GetComponent<Animator>();
            animator.enabled = false;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }


    void Update()
    {
        
    }

    public void changePicture()
    {
        if (stage + 1 < stages.Length)
        {
            stage++;
            spriteRenderer.sprite = stages[stage];
        }
        else if (stage + 1 == stages.Length)
        {
            finished = true;
            animator.enabled = true;
            animator.SetBool("ActivePortal", true);
        }
        //Debug.Log(stage)
    }
    public void teleport()
    {
        player.position = destination.position;
    }
}
