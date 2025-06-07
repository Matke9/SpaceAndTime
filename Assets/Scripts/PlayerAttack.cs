using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform fireballHolder;
    private GameObject[] fireballs;
    private PlayerController playerController;
    private int lastInput = 1;

    //private Animator anim;
    private PlayerController playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerController>();
        fireballs = new GameObject[fireballHolder.childCount];
        playerController = FindFirstObjectByType<PlayerController>();
        lastInput = playerController.GetLastInput();
        for (int i = 0; i < fireballHolder.childCount; i++)
        {
            fireballs[i] = fireballHolder.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer > attackCooldown)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        //anim.SetTrigger("attack");
        cooldownTimer = 0;
        lastInput = playerController.GetLastInput();
        if (lastInput == 1)
        {
            fireballs[FindFireball()].transform.position = new Vector3(firePoint.position.x, firePoint.position.y + 0.5f, firePoint.position.z);
        }
        else if (lastInput == 2)
        {
            fireballs[FindFireball()].transform.position = new Vector3(firePoint.position.x, firePoint.position.y - 0.5f, firePoint.position.z);
        }
        else if (lastInput == 3)
        {
            fireballs[FindFireball()].transform.position = new Vector3(firePoint.position.x - 0.5f, firePoint.position.y, firePoint.position.z);
        }
        else if (lastInput == 4)
        {
            fireballs[FindFireball()].transform.position = new Vector3(firePoint.position.x + 0.5f, firePoint.position.y, firePoint.position.z);
        }
        


        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(lastInput);
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
