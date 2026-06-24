using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] DungBall ball;

    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] InputAction chargeAction;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isGrounded = false;
    [SerializeField] Transform ballCheck;
    [SerializeField] float ballCheckRadius;
    [SerializeField] LayerMask ballMask;

    [SerializeField] bool isNearBall = false;
    [SerializeField] bool isChargingBall = false;
    [SerializeField] float chargeTime = 0;
    [SerializeField] float chargeScale;
    [SerializeField] float maxChargeTime;
    [SerializeField] GameObject chargeMeter;
    float chargeCooldown = 1.0f;
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
        chargeAction = actionMap.FindAction("Charge");

        chargeAction.started += OnChargeBall;
        chargeAction.canceled += OnReleaseBall;
    }
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

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

        //isGrounded = IsGrounded();
        //isNearBall = IsNearBall();
        if (isChargingBall)
        {
            ball.rb.linearVelocity = Vector2.zero;
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            Debug.Log("Charge time: " + chargeTime);
        }
        else
        {
            if (!moveAction.enabled) moveAction.Enable();
            chargeTime = 0;
        }
        UpdateChargeMeter();

        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        rb.linearVelocity = new Vector2(moveValue.x * playerData.walkSpeed, rb.linearVelocityY);
    }
    void OnChargeBall(InputAction.CallbackContext context)
    {
        if (!IsNearBall() || !IsGrounded()) return;
        moveAction.Disable();
        isChargingBall = true;
        Debug.Log("Charging ball..." + context);
    }
    void OnReleaseBall(InputAction.CallbackContext context)
    {
        if (!isChargingBall) return;
        isChargingBall = false;
        moveAction.Enable();
        float side = Mathf.Sign(transform.position.x - ball.transform.position.x);
        Vector2 dir = new Vector2(side, 0.3f).normalized;

        ball.rb.linearVelocity = -dir * chargeTime * chargeScale;
        chargeTime = 0;
        Debug.Log("Ball Launced..." + context);
    }
    void Jump()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.y = Mathf.Sqrt(2.0f * 9.81f * playerData.jumpHeight);
        rb.linearVelocity = velocity;
    }
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }
    bool IsNearBall()
    {
        return Physics2D.OverlapCircle(ballCheck.position, ballCheckRadius, ballMask);
    }
    void UpdateChargeMeter()
    {
        float t = Mathf.Clamp01(chargeTime / maxChargeTime);

        float height = t * 2f;

        RectTransform rt = chargeMeter.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        chargeMeter.GetComponent<Image>().color = GetChargeColor(t);
    }
    Color GetChargeColor(float t)
    {
        Color green = Color.green;
        Color yellow = Color.yellow;
        Color red = Color.red;

        if (t < 0.5f)
        {
            return Color.Lerp(green, yellow, t * 2f);
        }
        else
        {
            return Color.Lerp(yellow, red, (t - 0.5f) * 2f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ballCheck.position, ballCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    private void OnDestroy()
    {
        chargeAction.started -= OnChargeBall;
        chargeAction.canceled -= OnReleaseBall;
    }
}

