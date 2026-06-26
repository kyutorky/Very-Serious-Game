using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Main main;
    GameData gameData;
    GameData.PlayerData playerData;
    SFXController sfxController;
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
    [SerializeField] InputAction aimAction;


    //added by white so we can edit values in the inspector
    [SerializeField] bool useInspectorValues = false;
    [SerializeField] float walkSpeedOverride = 6f;
    [SerializeField] float dashSpeedOverride = 12f;
    [SerializeField] float jumpHeightOverride = 5f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isGrounded = false;
    [SerializeField] Transform ballCheck;
    [SerializeField] float ballCheckRadius;
    [SerializeField] LayerMask ballMask;

    [SerializeField] bool isNearBall = false;
    [SerializeField] public bool isChargingBall = false;
    [SerializeField] public float chargeTime = 0;
    [SerializeField] float chargeScale;
    [SerializeField] float maxChargeTime;
    [SerializeField] GameObject chargeMeter;
    [SerializeField] Vector2 aimVector;
    float aimVectorAngleWithWorldPosAxisX;
    Vector2 aimVectorClamped;
    Vector2 aimVectorNorm;
    Vector2 aimVectorNormScaled;
    Vector2 finalAimVector;
    [SerializeField] float aimRadius;

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
        this.sfxController = Main.Instance.sfxController;
        actionMap = actions.FindActionMap("Player");
        actionMap.Enable();
        moveAction = actionMap.FindAction("Move");
        sprintAction = actionMap.FindAction("Sprint");
        jumpAction = actionMap.FindAction("Jump");
        chargeAction = actionMap.FindAction("Charge");
        aimAction = actionMap.FindAction("Aim");

        chargeAction.started += OnChargeBall;
        chargeAction.canceled += OnReleaseBall;
    }
    void Update()
    {
        playerData.score = Mathf.Clamp(playerData.score, 0, 10000);
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
        if (sprintAction.IsPressed())   //added by white to allow player dash movement to work in the air
        {
            // This allows you to maintain or initiate dash multipliers while in the air!
            moveValue.x *= 1f;
        }
        if (isChargingBall)
        {
            ball.animator.speed = chargeTime;
            ball.rb.linearVelocity = Vector2.zero;
            ball.transform.position = transform.position + new Vector3(finalAimVector.x, finalAimVector.y);
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            Debug.Log("Charge time: " + chargeTime);
            CalculateAimVector();
            if (chargeTime >= maxChargeTime & sfxController.sources[6].isPlaying)
            {
                sfxController.sources[6].PlayDelayed(4);
            }
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

        float finalWalk = useInspectorValues ? walkSpeedOverride : (playerData != null ? playerData.walkSpeed : walkSpeedOverride);
        float finalDash = useInspectorValues ? dashSpeedOverride : (playerData != null ? playerData.dashSpeed : dashSpeedOverride);
        float currentMoveSpeed = sprintAction.IsPressed() ? finalDash : finalWalk;

        rb.linearVelocity = new Vector2(moveValue.x * currentMoveSpeed, rb.linearVelocityY);
    }
    void OnChargeBall(InputAction.CallbackContext context)
    {
        if (!IsNearBall() || !IsGrounded() || !ball.IsGrounded()) return;
        moveAction.Disable();
        isChargingBall = true;
        sfxController.sources[6].Play();
        Debug.Log("Charging ball..." + context);
    }
    void OnReleaseBall(InputAction.CallbackContext context)
    {
        if (!isChargingBall) return;
        isChargingBall = false;
        moveAction.Enable();
        sfxController.sources[6].Stop();
        sfxController.sources[7].PlayOneShot(sfxController.sources[7].clip);
        ball.rb.linearVelocity = finalAimVector * chargeTime * chargeScale;
        ball.rb.AddForce(finalAimVector * chargeTime * chargeScale);
        chargeTime = 0;
        Debug.Log("Ball Launced..." + context);
    }
    void Jump()
    {
        Vector2 velocity = rb.linearVelocity;

        // Choose between inspector value or configuration data
        float currentJumpHeight = useInspectorValues ? jumpHeightOverride : (playerData != null ? playerData.jumpHeight : jumpHeightOverride);

        velocity.y = Mathf.Sqrt(2.0f * 9.81f * currentJumpHeight);
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

    public void CalculateAimVector()
    {
        Vector2 pos = aimAction.ReadValue<Vector2>();
        aimVector = pos - new Vector2(transform.position.x, transform.position.y);
        aimVectorNorm = Vector2.Normalize(aimVector);
        aimVectorAngleWithWorldPosAxisX = (Mathf.Atan2(aimVectorNorm.y, Vector2.right.x)) * 180 / Mathf.PI;
        aimVectorClamped = new Vector2(aimVectorNorm.x, Mathf.Clamp(aimVectorNorm.y, 0.25f, 1));
        finalAimVector = aimVectorClamped * aimRadius;

        Debug.Log("Mouse Position: " + pos + "Aim Vector: " + pos + "Aim Vector Angle: " + aimVectorAngleWithWorldPosAxisX);

    }
    private void OnDrawGizmos()
    {

        if (isChargingBall)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(finalAimVector.x, finalAimVector.y));
        }

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

