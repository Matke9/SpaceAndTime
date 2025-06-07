using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float speed = 5;
    private float direction;
    private bool hit;
    private float lifetime;
    private Rigidbody2D rb;

    //private Animator anim;
    private CircleCollider2D boxCollider;

    private void Awake()
    {
        //anim = 
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        if (hit) return;

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
            return;
        hit = true;
        boxCollider.enabled = false;
        Enemy enemy;
        if (collision.TryGetComponent(out enemy))
        {
            enemy.Die();
        }
        Deactivate();
        //anim.SetTrigger("explode");
    }
    public void SetDirection()
    {
        lifetime = 0;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        Vector3 dirrection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dirrection.y, dirrection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        dirrection.z = 0;
        dirrection.Normalize();;
        
        rb.linearVelocity = dirrection * speed;
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
