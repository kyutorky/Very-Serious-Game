using UnityEngine;

public class DungBall : MonoBehaviour
{
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallMask;
    public bool isTouchingWall = false;
    public Rigidbody2D rb;
    SFXController sfxController;
    [SerializeField] float minSpeed = 0.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        this.sfxController = Main.Instance.sfxController;
    }
    void Update()
    {
        isTouchingWall = IsTouchingWall();
        float speed = rb.linearVelocity.magnitude;
        //rollingAudio.volume = 1;
        // rollingAudio.volume = Mathf.Clamp01(speed / 40f);

        if (speed > minSpeed)
        {
            if (!sfxController.sources[1].isPlaying)
            {
                sfxController.sources[1].Play();
                Debug.Log("Ball is rolling...");
            }

        }
        else
        {
            sfxController.sources[1].Stop();
        }
    }


    bool IsTouchingWall()
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

        if (impactSpeed > 2f)
        {
            Main.Instance.gameData.playerData.score -=
                Mathf.RoundToInt(impactSpeed);
            sfxController.sources[2].Play();
            OnLosePointsOnImpact(impactSpeed * 10);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Water"))
        {
            OnWaterSplash();
        }
    }
    public void OnLosePointsOnImpact(float val)
    {
        Main.Instance.gameData.playerData.score -= val;
        //oneshotAudio.volume = 0.5f;
        //oneshotAudio.PlayOneShot(losepointsSFX);
        sfxController.sources[3].Play();
    }
    public void OnWaterSplash()
    {
        Main.Instance.gameData.playerData.score -= 100;
        //oneshotAudio.volume = 1f;
        //oneshotAudio.PlayOneShot(watersplashSFX);
        sfxController.sources[5].Play();
    }
}
