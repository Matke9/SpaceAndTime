using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float time_value = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stoppingDistance = 0.1f; // Distance at which enemy stops
    [SerializeField] private LayerMask obstacleLayer;
    

    private Vector3 lastKnownPlayerPosition;
    private bool canSeePlayer;
    private bool isMovingToLastPosition;
    private bool hasReachedLastPosition;
    private Rigidbody2D rb;
    private float sideWoddle = 2f;
    private float currentWoddle;
    private float yRotation = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        lastKnownPlayerPosition = transform.position;
        hasReachedLastPosition = true;
    }

    private void Update()
    {
        if (player == null) return;
        CheckPlayerVisibility();

    }

    private void FixedUpdate()
    {
        if (GameManager.pausedGame == true)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            MoveEnemy();
        }
    }

    private void CheckPlayerVisibility()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                directionToPlayer.normalized,
                distanceToPlayer,
                obstacleLayer
            );

            if (hit.collider == null)
            {
                canSeePlayer = true;
                lastKnownPlayerPosition = player.position;
                isMovingToLastPosition = false;
                hasReachedLastPosition = false;
                Debug.DrawLine(transform.position, player.position, Color.green);
            }
            else
            {
                canSeePlayer = false;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        else
        {
            canSeePlayer = false;
        }

        if (!canSeePlayer && !isMovingToLastPosition && !hasReachedLastPosition)
        {
            isMovingToLastPosition = true;
        }
    }

    private void MoveEnemy()
    {
        if (hasReachedLastPosition && !canSeePlayer)
        {
            transform.rotation = Quaternion.Euler(0,0,0);;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition = canSeePlayer ? player.position : lastKnownPlayerPosition;
        Vector2 direction = (targetPosition - rb.position).normalized;
        float distanceToTarget = Vector2.Distance(rb.position, targetPosition);

        if (isMovingToLastPosition && distanceToTarget < stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            hasReachedLastPosition = true;
            isMovingToLastPosition = false;
            return;
        }

        rb.linearVelocity = direction * moveSpeed;

        // Okretanje sprite-a
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(-(Mathf.Sign(direction.x)*0.67f), 0.67f, 1);
        }
        /*
        if (direction.x < 0)
        {
            Debug.Log("Levo");
            yRotation = 0;
        }
        else if (direction.x > 0)
        {
            Debug.Log("Desno");
            yRotation = 180;
        }
        */
        currentWoddle += sideWoddle;;

        if (Mathf.Abs(currentWoddle) > 10f)
        {

            sideWoddle *= -1;
            currentWoddle = Mathf.Sign(currentWoddle) * 10f;
        }
        transform.rotation = Quaternion.Euler(0,0,currentWoddle);;
    }

    public void Die()
    {
        GameTimeManager.AddTime(time_value);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}