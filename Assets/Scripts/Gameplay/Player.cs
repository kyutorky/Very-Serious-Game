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
    void Start()
    {
        main = Main.Instance;
        camera = main.camera;
        camera.target = transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        actionMap = actions.FindActionMap("Player");
        actionMap.Enable();
        moveAction = actionMap.FindAction("Move");
        sprintAction = actionMap.FindAction("Sprint");
        Initialize();
    }
    private void Initialize()
    {
        gameData = Main.Instance.gameData;
        playerData = gameData.playerData;
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

        if (sprintAction.IsPressed())
        {
            moveValue *= playerData.dashSpeed;
        }

        rb.linearVelocity = moveValue;
    }
    void takeDamage(float val)
    {
        gameData.playerData.healthPoints = val;
    }
}
