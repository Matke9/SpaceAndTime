using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Following Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("No target set for camera! Please assign a target in the inspector or tag your player as 'Player'");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Izračunaj željenu poziciju
        Vector3 desiredPosition = target.position + offset;
        
        // Ako koristimo granice, ograniči željenu poziciju
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Smooth kretanje ka željenoj poziciji
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / smoothSpeed
        );

        // Postavi novu poziciju kamere
        transform.position = smoothedPosition;
    }

    // Pomoćna funkcija za postavljanje novog targeta
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Vizuelni prikaz granica u editoru
    private void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
            new Vector3(maxX - minX, maxY - minY, 0)
        );
    }
}