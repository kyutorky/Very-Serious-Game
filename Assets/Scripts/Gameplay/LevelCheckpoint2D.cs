using UnityEngine;

public class LevelCheckpoint2D : MonoBehaviour
{
    [Header("Teleport Destination")]
    [Tooltip("The position where the player and ball will be sent in the next level section.")]
    public Vector3 nextLevelSpawnPoint;

    [Header("Tracking Parameters")]
    public float checkRadius = 1.5f;
    public LayerMask playerMask;
    public LayerMask ballMask;

    [Header("Visual Debugging")]
    public bool showGizmos = true;

    private void Update()
    {
        // Check if both objects are simultaneously inside the checkpoint zone
        bool isPlayerHere = Physics2D.OverlapCircle(transform.position, checkRadius, playerMask);
        bool isBallHere = Physics2D.OverlapCircle(transform.position, checkRadius, ballMask);

        if (isPlayerHere && isBallHere)
        {
            TeleportToNextLevel();
        }
    }

    private void TeleportToNextLevel()
    {
        // Find the active Player and DungBall instances in the scene
        Player player = Object.FindFirstObjectByType<Player>();
        DungBall ball = Object.FindFirstObjectByType<DungBall>();

        if (player != null)
        {
            // Reset velocity to prevent carrying momentum over, then move
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

            player.transform.position = nextLevelSpawnPoint;
        }

        if (ball != null)
        {
            // Reset velocity so the ball drops statically at the start of the next section
            if (ball.rb != null) ball.rb.linearVelocity = Vector2.zero;

            // Offset the ball slightly so it doesn't spawn exactly inside the player
            ball.transform.position = nextLevelSpawnPoint + new Vector3(3.5f, 0f, 0f);
        }

        Debug.Log("Player and Ball successfully teleported to the next level area.");
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draws the detection zone in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // Draws the destination point in cyan
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(nextLevelSpawnPoint, 0.5f);
        Gizmos.DrawLine(transform.position, nextLevelSpawnPoint);
    }
}
