using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    Main main;
    GameData gameData;
    [SerializeField] Button button;

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
    }
    public void OnButtonPressed()
    {
        gameData.playerData.healthPoints -= 50;
    }
}
