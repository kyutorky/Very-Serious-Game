using UnityEngine;

public class DungBall : MonoBehaviour
{
    [SerializeField] Player player;
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallMask;
    public bool isTouchingWall = false;
    public Rigidbody2D rb;
    SFXController sfxController;
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] public Animator animator;
    [SerializeField] ParticleSystem vfxImpact;
    [SerializeField] ParticleSystem vfxRolling;
    [SerializeField] ParticleSystem vfxWaterSplash;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isGrounded = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vfxImpact.Pause();
        this.sfxController = Main.Instance.sfxController;
    }
    void Update()
    {
        isTouchingWall = IsTouchingWall();
        isGrounded = IsGrounded();
        float speed = rb.linearVelocity.magnitude;
        if (animator != null)
        {
            if (player.isChargingBall)
            {
                animator.speed = player.chargeTime * 1.2f;
            }
            else
            {
                animator.speed = speed > minSpeed ? speed * 0.2f : 0f;
            }
        }

        if (speed > minSpeed && IsGrounded())
        {

            if (!sfxController.sources[1].isPlaying)
            {
                sfxController.sources[1].Play();
                Debug.Log("Ball is rolling...");
                vfxRolling.Play();

            }

        }
        else
        {
            sfxController.sources[1].Stop();
            vfxRolling.Stop();
        }
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }

    public bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallMask);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Ball Collided with {collision.collider.name}");
        if (((1 << collision.gameObject.layer) & wallMask) == 0)
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        Debug.Log($"Wall hit at speed: {impactSpeed}");

        if (impactSpeed > 4f)
        {
            Main.Instance.gameData.playerData.score -=
                Mathf.RoundToInt(impactSpeed);
            sfxController.sources[2].Play();
            OnLosePointsOnImpact(500);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Water"))
        {
            OnWaterSplash();
        }
        if (collision.gameObject.tag.Equals("ScorePickup"))
        {
            Object.Destroy(collision.gameObject);
            sfxController.sources[3].pitch = 1.5f;
            sfxController.sources[3].Play();
            Main.Instance.gameData.playerData.score += 1000;
        }
    }
    public void OnLosePointsOnImpact(float val)
    {
        Main.Instance.gameData.playerData.score -= val;
        sfxController.sources[3].pitch = 0.5f;
        sfxController.sources[3].Play();
        vfxImpact.Emit(30);
    }
    public void OnWaterSplash()
    {
        Main.Instance.gameData.playerData.score -= 100;
        sfxController.sources[5].Play();
        vfxWaterSplash.Emit(10);
    }
}
