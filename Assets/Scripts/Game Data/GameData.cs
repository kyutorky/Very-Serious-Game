using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public SceneControllerData sceneControllerData { get; set; }
    [SerializeField] public PlayerData playerData;
    public GameplayData gameplayData { get; set; }
    public AudioSettings audioSettings { get; set; }

    public Dictionary<string, Action<float>> dataSetters;
    DevCommandRegistry cmdRegistry;

    public GameData()
    {
        sceneControllerData = new SceneControllerData();
        playerData = new PlayerData();
        gameplayData = new GameplayData();
        audioSettings = new AudioSettings();
    }
    public void Initialize(DevCommandRegistry cmdRegistry)
    {
        //  this.cmdRegistry = cmdRegistry;

        //  this.playerData = new PlayerData();
        // this.gameplayData = new GameplayData();
        // this.audioSettings = new AudioSettings();
        dataSetters = new Dictionary<string, Action<float>>();
        dataSetters.Add("health", playerData.setHealth);
        dataSetters.Add("walkspeed", playerData.setWalkSpeed);
        cmdRegistry.AddCommand(new DevCommand("SetterCmd", "set", OnDataChange));
    }
    public string OnDataChange(string[] args)
    {
        if (args.Length < 2)
        {
            return "Not enough arguments for set cmd.";
        }
        //args[0] = settername, args[1] = newvalue
        if (dataSetters.TryGetValue(args[0], out Action<float> setter))
        {
            if (float.TryParse(args[1], out var result))
            {
                setter.Invoke(result);
            }
            else
            {
                return $"Invalid input for {args[0]}";
            }
            return $"{args[0]} changed to {args[1]}";
        }
        else
        {
            return $"Unable to find setter for {args[0]};";
        }
    }

    public override string ToString()
    {
        return playerData.ToString() + audioSettings.ToString();
    }
    public class SceneControllerData
    {
        public List<string> persistentScenes;
        public List<string> mainmenuScenes;
        public List<string> gameplayScenes;
        public List<string> gameoverScenes;
        public SceneControllerData()
        {
            persistentScenes = new List<string>();
            mainmenuScenes = new List<string>();
            gameplayScenes = new List<string>();
            gameoverScenes = new List<string>();
        }
    }

    [Serializable]
    public class PlayerData
    {
        [SerializeField] public float healthPoints = 0;
        [SerializeField] public float walkSpeed = 0;
        [SerializeField] public float dashSpeed = 0;
        [SerializeField] public float score = 0;
        public void setHealth(float val) { this.healthPoints = val; }
        public void setWalkSpeed(float val) { this.walkSpeed = val; }
        public override string ToString()
        {
            return "PlayerData: \n" +
                $"\tHP {healthPoints}\twalkSpd {walkSpeed}\tdashSpd {dashSpeed}\tscore {score}\n";
        }
    }
    public class GameplayData
    {
        public float targetScore = 0;
        public float timeLimitInSeconds = 0;
        public float timeScale = 0;
        public override string ToString()
        {
            return "GameConditions: \n" +
                $"\ttargetScore {targetScore}\ttimeLimit(s) {timeLimitInSeconds}\ttimeScale {timeScale}\n";
        }
    }
    public class AudioSettings
    {
        public float masterVolume = 0;
        public float bgmVolume = 0;
        public float sfxVolume = 0;
        public override string ToString()
        {
            return $"AudioSettings: \n" +
                $"\tmstrVol {masterVolume}\tbgmVol {bgmVolume}\tsfxVol {sfxVolume}\n";
        }
    }



}
