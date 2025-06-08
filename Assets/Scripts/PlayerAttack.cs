using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform fireballHolder;
    [SerializeField] private AudioSource shootSound;
    private GameObject[] fireballs;
    private PlayerController playerController;

    //private Animator anim;
    private PlayerController playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerController>();
        fireballs = new GameObject[fireballHolder.childCount];
        playerController = FindFirstObjectByType<PlayerController>();
        for (int i = 0; i < fireballHolder.childCount; i++)
        {
            fireballs[i] = fireballHolder.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer > attackCooldown && GameManager.pausedGame == false)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        shootSound.Play();
        //anim.SetTrigger("attack");
        if (GameTimeManager.ReduceTime(5) && GameManager.pausedGame == false)
        {
            cooldownTimer = 0;
            fireballs[FindFireball()].transform.position =
                new Vector3(firePoint.position.x, firePoint.position.y, firePoint.position.z);
            fireballs[FindFireball()].GetComponent<Projectile>().SetDirection();
        }
    }
    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
