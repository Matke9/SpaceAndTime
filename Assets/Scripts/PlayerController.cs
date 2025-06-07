using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool wasdMovement;
    private Transform playerTransform;
    private Rigidbody2D playerRigidbody2D;
    private int lastInput = 1;
    
    public int GetLastInput()
    {
        return lastInput;
    }
    
    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool pressedUp;
        bool pressedLeft;
        bool pressedDown;
        bool pressedRight;
        int moveUpDown = 0;
        int moveLeftRight = 0;
        if (wasdMovement)
        {
            pressedUp = Input.GetKey(KeyCode.W);
            pressedLeft = Input.GetKey(KeyCode.A);
            pressedDown = Input.GetKey(KeyCode.S);
            pressedRight = Input.GetKey(KeyCode.D);
        }
        else
        {
            pressedUp = Input.GetKey(KeyCode.UpArrow);
            pressedLeft = Input.GetKey(KeyCode.LeftArrow);
            pressedDown = Input.GetKey(KeyCode.DownArrow);
            pressedRight = Input.GetKey(KeyCode.RightArrow);
        }

        if (pressedUp && !pressedDown)
        {
            moveUpDown = 1;
        }
        else if (!pressedUp && pressedDown)
        {
            moveUpDown = -1;
        }
        if (pressedLeft && !pressedRight)
        {
            moveLeftRight = -1;
        }
        else if (!pressedLeft && pressedRight)
        {
            moveLeftRight = 1;
        }

        if (pressedLeft && Mathf.RoundToInt(Mathf.Abs(playerTransform.rotation.y)) == 1)
        {
            
            playerTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (pressedRight && playerTransform.rotation.y == 0f)
        {
            playerTransform.rotation = Quaternion.Euler(0, 180, 0);
        }

        playerRigidbody2D.linearVelocity = new Vector2(moveLeftRight * moveSpeed, moveUpDown * moveSpeed);
        
        if (pressedUp)
        {
            lastInput = 1;
        }
        else if (pressedDown)
        {
            lastInput = 2;
        }
        else if (pressedLeft)
        {
            lastInput = 3;
        }
        else if (pressedRight)
        {
            lastInput = 4;
        }

    }
}
