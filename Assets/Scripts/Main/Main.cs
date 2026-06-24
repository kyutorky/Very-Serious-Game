using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }
    [SerializeField] public CameraFollow camera;
    [SerializeField] public GameSceneController gameSceneController;
    [SerializeField] public SFXController sfxController;
    [SerializeField] GameObject sfxSourcesGO;
    public DevCommandRegistry cmdRegistry;
    public InGameDevConsole devConsole;
    [SerializeField] public GameData gameData;
    public string configPath;
    private void Awake()
    {
        Instance = GetComponent<Main>();
        if (Instance == null)
        {
            Debug.Log("No instance of Main.");
        }
        else
        {
            cmdRegistry = new DevCommandRegistry();
            //configPath = Path.Combine(Application.streamingAssetsPath, "config.json");
            TextAsset config = Resources.Load<TextAsset>("gameconfig");

            gameData = GetGameDataFromConfigJson(config.text);
            gameData.Initialize(cmdRegistry);

            gameSceneController = new GameSceneController();
            sfxController = new SFXController(sfxSourcesGO);
            gameSceneController.Initialize(gameData);
            StartCoroutine(gameSceneController.LoadScenes(new List<string>(), gameSceneController.persistentScenes));
            StartCoroutine(gameSceneController.LoadScenes(new List<string>(), gameSceneController.mainmenuScenes));
            cmdRegistry.AddCommand(new DevCommand("Reset Game", "restart", OnGameReset));
            cmdRegistry.AddCommand(new DevCommand("loadmainmenu", "loadmainmenu", OnMainmenuLoad));
            cmdRegistry.AddCommand(new DevCommand("loadgameplay", "loadgameplay", OnGameplayLoad));
            cmdRegistry.AddCommand(new DevCommand("loadgameover", "loadgameover", OnGameoverLoad));
        }
    }
    public string OnMainmenuLoad(string[] args)
    {
        StartCoroutine(TransitionScenes(gameSceneController.current, gameSceneController.mainmenuScenes));
        return "Mainmenu loaded";
    }
    public string OnGameplayLoad(string[] args)
    {

        gameData.playerData.score = 1000;  //temporary
        StartCoroutine(TransitionScenes(gameSceneController.current, gameSceneController.gameplayScenes));
        return "Gameplay loaded";
    }
    public string OnGameoverLoad(string[] args)
    {
        StartCoroutine(TransitionScenes(gameSceneController.current, gameSceneController.gameoverScenes));
        return "Gameover loaded";
    }
    public IEnumerator TransitionScenes(List<string> current, List<string> next)
    {
        yield return StartCoroutine(gameSceneController.LoadScenes(current, next));
        yield return StartCoroutine(gameSceneController.UnloadScenes(gameSceneController.previous));
    }
    public string OnGameReset(string[] args)
    {
        gameData = new GameData();
        return "Game reset.";
    }
    public GameData GetGameDataFromConfigJson(string configText)
    {
        GameData gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(configText);
        return gameData;
    }
}
public class DevCommandRegistry
{
    public Dictionary<string, DevCommand> commands;
    public DevCommandRegistry()
    {
        commands = new Dictionary<string, DevCommand>();
    }
    public void AddCommand(DevCommand command)
    {
        if (command != null) commands.Add(command.registryKey, command);
        else Debug.Log($"Unable to add command {command.name} to cmd registry.");
    }
    public DevCommand GetCommand(string inputKey)
    {
        if (commands.TryGetValue(inputKey, out var command))
        {
            return command;
        }
        else
        {
            return null;
        }
    }
}
public class DevCommand
{
    public string name { get; }
    public string registryKey { get; }
    public Func<string[], string> cmdFunc { get; }
    public DevCommand(string name, string registryKey, Func<string[], string> cmdFunc)
    {
        this.name = name;
        this.registryKey = registryKey;
        this.cmdFunc = cmdFunc;
    }
    public string Execute(string[] args)
    {
        string result = cmdFunc.Invoke(args);
        return result;
    }
}
