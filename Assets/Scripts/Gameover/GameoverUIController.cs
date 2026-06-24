using UnityEngine;
using UnityEngine.UI;

public class GameoverUIController : MonoBehaviour
{
    Main main;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] Button button;

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
    }

    public void OnButtonPressed()
    {
        Main.Instance.sfxController.sources[4].Play();
        Main.Instance.OnGameplayLoad(null);
    }
}
