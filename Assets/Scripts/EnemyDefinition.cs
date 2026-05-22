using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Space and Time/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float detectionRange = 10f;

    [Header("Attack")]
    public float attackRange = 0.3f;
    public float attackCooldown = 2f;

    [Header("Time")]
    public float timeRefundOnKill = 5f;

    [Header("Collision")]
    public LayerMask obstacleMask;
}
