using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameSceneController
{
    public List<string> persistentScenes;
    public List<string> mainmenuScenes;
    public List<string> gameplayScenes;
    public List<string> gameoverScenes;
    public Dictionary<string, List<string>> sceneGroups;

    [SerializeField] public List<string> previous;
    [SerializeField] public List<string> current;
    public GameSceneController()
    {
        persistentScenes = new List<string>();
        mainmenuScenes = new List<string>();
        gameplayScenes = new List<string>();
        gameoverScenes = new List<string>();
        //  previous = new List<string>();
        //current = new List<string>();
        sceneGroups = new Dictionary<string, List<string>>();
    }
    public void Initialize(GameData gameData)
    {
        GameData.SceneControllerData scData = gameData.sceneControllerData;
        this.persistentScenes = scData.persistentScenes;
        this.mainmenuScenes = scData.mainmenuScenes;
        this.gameplayScenes = scData.gameplayScenes;
        this.gameoverScenes = scData.gameoverScenes;
        this.sceneGroups.Add("mainmenu", this.mainmenuScenes);
        this.sceneGroups.Add("gameplay", this.gameplayScenes);
        this.sceneGroups.Add("gameover", this.gameoverScenes);
    }
    public IEnumerator LoadScenes(List<string> current, List<string> next)
    {
        this.previous = current;

        if (next.Count > 0)
        {
            foreach (string name in next)
            {
                Scene s = SceneManager.GetSceneByName(name);
                if (s != null)
                {
                    AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                    while (!sceneLoadOperation.isDone)
                    {
                        yield return null;
                    }
                    Debug.Log($"Loaded the scene called {name}");
                }
                else
                {
                    Debug.Log($"Unable to load the scene called {name}.");
                }
            }

            // yield return new WaitForSeconds(1);
        }
        this.current = next;
        yield return null;
        // yield return new WaitForSeconds(1);
    }
    public IEnumerator UnloadScenes(List<string> sceneNames)
    {
        //yield return new WaitForSeconds(1);
        if (sceneNames.Count > 0)
        {
            foreach (string name in sceneNames)
            {
                Scene s = SceneManager.GetSceneByName(name);
                if (s != null)
                {
                    AsyncOperation sceneUnloadOperation = SceneManager.UnloadSceneAsync(name, UnloadSceneOptions.None);
                    while (!sceneUnloadOperation.isDone)
                    {
                        yield return null;
                    }
                    Debug.Log($"The scene {name} is unloaded.");
                }
                else
                {
                    Debug.Log($"Unable to unload the scene called {name}.");
                }
                // yield return new WaitForSeconds(1);
            }
        }
        yield return null;
    }
}
