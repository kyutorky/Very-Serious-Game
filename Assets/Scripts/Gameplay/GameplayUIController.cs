using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    Main main;
    GameData gameData;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] DungBall ball;
    [SerializeField] Button button;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text ballStatText;
    [SerializeField] Rigidbody2D ballRb;
    void Start()
    {
        main = Main.Instance;
        gameData = main.gameData;
        if (main == null)
        {
            Debug.Log("No instance of Main for Main Menu Controller.");
        }
        else if (button != null)
        {
            button.onClick.AddListener(OnButtonPressed);
        }
        else
        {
            Debug.Log("No Start Button for Main Menu Controller.");
        }
        ballRb = ball.GetComponent<Rigidbody2D>();
        audio.clip = clickSFX;
    }
    private void Update()
    {
        OnUpdateScoreText(gameData.playerData.score);
        OnUpdateBallStatText();
    }
    public void OnButtonPressed()
    {
        audio.Play();
        Main.Instance.OnGameplayLoad(null);
    }
    public void OnUpdateScoreText(float value)
    {
        scoreText.text = $"{value}";
    }
    public void OnUpdateBallStatText()
    {

        ballStatText.text = $"Ball velocity: {ballRb.linearVelocity.ToString()}";
    }
}
