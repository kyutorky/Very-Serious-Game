using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameoverUIController : MonoBehaviour
{
    Main main;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] Button button;
    [SerializeField] TMP_Text message;

    void Start()
    {
        main = Main.Instance;

        if (main == null)
        {
            Debug.Log("No instance of Main for GameoverUIController.");
        }
        else if (button != null)
        {
            button.onClick.AddListener(OnButtonPressed);
        }
        else
        {
            Debug.Log("No Restart Button for GameoverUIController.");
        }
        audio.clip = clickSFX;
        message.text = $"Game Over\nScore: {main.gameData.playerData.score}";
    }

    public void OnButtonPressed()
    {
        Main.Instance.sfxController.sources[4].Play();
        Main.Instance.OnGameplayLoad(null);
    }
}
