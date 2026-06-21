using UnityEngine;

public class DungBall : MonoBehaviour
{
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallMask;
    public bool isTouchingWall = false;

    void Start()
    {

    }
    void Update()
    {
        isTouchingWall = IsTouchingWall();
    }

    bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallMask);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Ball Collided with {collision.collider.name}");
    }
}
