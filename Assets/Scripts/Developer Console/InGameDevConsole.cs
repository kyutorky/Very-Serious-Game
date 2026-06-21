using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameDevConsole : MonoBehaviour
{
    [SerializeField] InputActionAsset actionAsset;
    InputActionMap actionMap;
    InputAction toggleOverlay;
    bool isDevOverlayOn = false;
    [SerializeField] CanvasGroup canvasGroup;
    int fpsCount = 0;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text outputField;
    [SerializeField] TMP_Text gameDataDisplay;
    [SerializeField] TMP_Text overlay;
    DevCommandRegistry cmdRegistry;
    GameData gameData;
    public Action<string> gameDataDisplayText;
    public bool isDataDisplayOn = false;

    float timer = 0f;
    float delay = 2f;
    void Start()
    {
        Initialize(Main.Instance.cmdRegistry, Main.Instance.gameData);
    }
    private void Update()
    {
        if (toggleOverlay.WasPressedThisFrame())
        {
            isDevOverlayOn = !isDevOverlayOn;
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                isDevOverlayOn = !isDevOverlayOn;
                timer = 0f;
            }
        }

        if (isDevOverlayOn)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            if (isDataDisplayOn)
            {
                gameDataDisplay.gameObject.transform.parent.gameObject.SetActive(isDataDisplayOn);
                gameDataDisplay.text = gameData.ToString();

                overlay.text = $"FPS: {fpsCount}";
            }
            else
            {
                gameDataDisplay.gameObject.transform.parent.gameObject.SetActive(isDataDisplayOn);
                gameDataDisplay.text = "";
            }
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void Initialize(DevCommandRegistry cmdRegistry, GameData gameData)
    {
        this.cmdRegistry = cmdRegistry;
        this.gameData = gameData;
        cmdRegistry.AddCommand(new DevCommand("Show data display", "showdata", OnDataDisplayToggle));
        inputField.onSelect.AddListener(OnEnterCommand);
        inputField.onSubmit.AddListener(OnSubmitCommand);
        StartCoroutine(GetFPS());
        actionMap = actionAsset.FindActionMap("Player");
        toggleOverlay = actionMap.FindAction("DevOverlay");
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnEnterCommand(string c)
    {
        Debug.Log("Entering Command..." + c + "\n");
    }
    public void OnSubmitCommand(string input)
    {
        inputField.text = "";
        Debug.Log($"Command submitted: {input}\n ");

        string[] inputArr = input.Split(" ");
        string[] args = new string[inputArr.Length - 1];
        string key = inputArr[0];
        if (inputArr.Length > 1) args = inputArr[1..inputArr.Length];
        Debug.Log($"Command args: [{string.Join(", ", args)}]");
        DevCommand selectedCmd = cmdRegistry.GetCommand(key);

        if (selectedCmd != null)
        {
            outputField.text = $"-> {selectedCmd.Execute(args)}\n";
        }
        else
        {
            outputField.text = $"-> Command does not exist.\n";
        }
    }
    public string OnDataDisplayToggle(string[] args)
    {
        if (args.Length == 0)
        {
            return "No arguments for showdata.";
        }
        else if (args[0].Equals("off"))
        {
            isDataDisplayOn = false;
            return "Data display is off.";
        }
        else if (args[0].Equals("on"))
        {
            isDataDisplayOn = true;
            return "Data display is on.";
        }
        else return "Invalid argument for showdata.";
    }
    public IEnumerator GetFPS()
    {
        while (true)
        {
            fpsCount = (int)(1f / Time.unscaledDeltaTime);
            yield return new WaitForSeconds(.1f);
        }
    }
}
