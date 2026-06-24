using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    Main main;
    [SerializeField] Button startGameButton;
    private void Start()
    {
        main = Main.Instance;

        if (main == null)
        {
            Debug.Log("No instance of Main for Main Menu Controller.");
        }
        else if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGamePressed);
        }
        else
        {
            Debug.Log("No Start Button for Main Menu Controller.");
        }
    }
    public void OnStartGamePressed()
    {
        Main.Instance.sfxController.sources[4].Play();
        main.OnGameplayLoad(null);
        Debug.Log("Start Button pressed.");
    }

}
