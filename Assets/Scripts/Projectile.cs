using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifetime;

    //private Animator anim;
    private CircleCollider2D boxCollider;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        if (direction == 1)
        {
            transform.Translate(0, movementSpeed, 0);
        }
        else if (direction == 2)
        {
            
            transform.Translate(0, -movementSpeed, 0);
        }
        else if (direction == 3)
        {
            
            transform.Translate(-movementSpeed, 0, 0);
        }
        else if (direction == 4)
        {
            
            transform.Translate(movementSpeed, 0, 0);
        }

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        Deactivate();
        //anim.SetTrigger("explode");
    }
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        float localScaleY = transform.localScale.y;
        
        if (direction == 1)
        {
            transform.localScale = new Vector3(transform.localScale.x, localScaleY, transform.localScale.z);
        }
        else if (direction == 2)
        {
            transform.localScale = new Vector3(transform.localScale.x, -localScaleY, transform.localScale.z);
        }
        else if (direction == 3)
        {
            transform.localScale = new Vector3(-localScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (direction == 4)
        {
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }
        
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
