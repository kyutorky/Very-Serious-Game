using UnityEngine;
using UnityEngine.SceneManagement; // Allows us to restart the level

public class DeathZoneHandler : MonoBehaviour
{
    // This runs automatically because "Is Trigger" is enabled on the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Did the player fall through?
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into the pit!");
            RestartLevel();
        }
        
        // Did the ball fall through?
        else if (collision.CompareTag("Ball"))
        {
            Debug.Log("The ball was dropped!");
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        // Instantly reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
