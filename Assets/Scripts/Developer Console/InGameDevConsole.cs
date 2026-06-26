using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] GameObject sfxPanel;
    [SerializeField] GameObject sfxSliderPrefab;
    [SerializeField] GameObject sfxButtonPrefab;
    [SerializeField] List<Slider> sfxSliders;
    [SerializeField] List<Button> sfxButtons;
    DevCommandRegistry cmdRegistry;
    GameData gameData;
    SFXController sfxController;
    public Action<string> gameDataDisplayText;
    public bool isDataDisplayOn = false;

    float timer = 0f;
    float delay = 2f;
    void Start()
    {
        Initialize(Main.Instance.cmdRegistry, Main.Instance.gameData, Main.Instance.sfxController);

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
            ReadSFXSliderValues();
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void Initialize(DevCommandRegistry cmdRegistry, GameData gameData, SFXController sfxController)
    {
        this.cmdRegistry = cmdRegistry;
        this.gameData = gameData;
        this.sfxController = sfxController;
        cmdRegistry.AddCommand(new DevCommand("Show data display", "showdata", OnDataDisplayToggle));
        inputField.onSelect.AddListener(OnEnterCommand);
        inputField.onSubmit.AddListener(OnSubmitCommand);
        StartCoroutine(GetFPS());
        actionMap = actionAsset.FindActionMap("Player");
        toggleOverlay = actionMap.FindAction("DevOverlay");
        canvasGroup = GetComponent<CanvasGroup>();
        RenderSFXPanel();
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

    public void RenderSFXPanel()
    {
        if (sfxPanel == null)
        {
            Debug.Log("No panel assigned for SFX");
            return;
        }

        for (int i = 0; i < sfxController.sources.Count; i++)
        {
            GameObject slider = GameObject.Instantiate(sfxSliderPrefab);
            GameObject button = GameObject.Instantiate(sfxButtonPrefab);
            if (slider == null)
            {
                Debug.Log("SFX slider is null");
            }
            slider.transform.SetParent(sfxPanel.transform, false);
            slider.transform.localPosition = new Vector3(0, -i * 30 - 20, 0);
            button.transform.SetParent(sfxPanel.transform, false);
            button.transform.localPosition = new Vector3(-100, -i * 30 - 20, 0);
            sfxSliders.Add(slider.GetComponent<Slider>());
            sfxButtons.Add(button.GetComponent<Button>());
            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => OnSFXClipPlay(index));
        }

    }
    public void OnSFXClipPlay(int i)
    {
        if (i != 0)
        {
            Main.Instance.sfxController.sources[i].Stop();
            Main.Instance.sfxController.sources[i].PlayOneShot(Main.Instance.sfxController.sources[i].clip);
        }

    }
    public void ReadSFXSliderValues()
    {
        List<AudioSource> sources = Main.Instance.sfxController.sources;
        if (sfxSliders.Count == 0)
        {
            Debug.Log("No slider values to read.");
            return;
        }
        for (int i = 0; i < sfxSliders.Count; i++)
        {
            sources[i].volume = sfxSliders[i].value;
            sfxSliders[i].transform.GetComponent<TMP_Text>().text = $"\n{sources[i].name}: {(float)Math.Truncate(sources[i].volume * 100f) / 100f}";

        }
    }

}
