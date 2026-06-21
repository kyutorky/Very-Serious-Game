using UnityEngine;

public class DeathZoneHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if the player or the ball dropped below the level screen boundary
        if (collision.CompareTag("Player") || collision.CompareTag("Ball"))
        {
            Debug.Log($"{collision.gameObject.name} hit the bottom boundary. Loading Game Over screen...");
            TriggerGameOverTransition();
        }
    }

    private void TriggerGameOverTransition()
    {
        if (Main.Instance != null && Main.Instance.gameSceneController != null)
        {
            // Bypass the broken OnGameoverLoad check by running the working coroutine directly
            var controller = Main.Instance.gameSceneController;
            Main.Instance.StartCoroutine(Main.Instance.TransitionScenes(controller.current, controller.gameoverScenes));
        }
        else
        {
            // Safety fallback message in case the scene was launched without the core architecture running
            Debug.LogError("Main.Instance or GameSceneController not found! Cannot route to Game Over scene.");
        }
    }
}
