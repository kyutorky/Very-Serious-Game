using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Main main;
    GameData gameData;
    GameData.PlayerData playerData;
    [SerializeField] InputActionAsset actions;
    [SerializeField] InputActionMap actionMap;

    CameraFollow camera;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction jumpAction;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;
    public bool isGrounded = false;
    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        gameData = Main.Instance.gameData;
        playerData = gameData.playerData;
        main = Main.Instance;
        camera = main.camera;
        camera.target = transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        actionMap = actions.FindActionMap("Player");
        actionMap.Enable();
        moveAction = actionMap.FindAction("Move");
        sprintAction = actionMap.FindAction("Sprint");
        jumpAction = actionMap.FindAction("Jump");
    }
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Debug.Log(moveValue);
        if (moveValue != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("xInput", moveValue.x);
            animator.SetFloat("xInput", moveValue.y);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (sprintAction.IsPressed() && IsGrounded())
        {
            moveValue *= playerData.dashSpeed;
        }
        isGrounded = IsGrounded();
        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }
        rb.linearVelocity = new Vector2(moveValue.x * playerData.walkSpeed, rb.linearVelocityY);
    }
    void Jump()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.y = Mathf.Sqrt(2.0f * 9.81f * playerData.jumpHeight);
        rb.linearVelocity = velocity;
        Debug.Log("Player jumped.");
    }
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }
}
