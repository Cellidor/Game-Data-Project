using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class GameSaveManager : MonoBehaviour {

    public string gameDataFilename = "game-data.json";
    private string gameDataFilePath;
    public List<string> saveItems;
    public bool firstPlay = true;

    // Singleton pattern from:
    // http://clearcutgames.net/home/?p=437

    // Static singleton property
    public static GameSaveManager Instance { get; private set; }


    void Awake() {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
            return;
        }


        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);

        // Set the location to the current application's data path, along with "/game-data.json"
        gameDataFilePath = Application.dataPath + "/" + gameDataFilename;
        saveItems = new List<string>();
    }


    public void AddObject(string item) {
        // pass a given string "item" to add to the list of save items.
        saveItems.Add(item);
    }

    public void Save() {
        // Clear old save items
        saveItems.Clear();

        // Fina all game objects that use the base class "Save". Such items contain save functionality.
        Save[] saveableObjects = GameObject.FindObjectsOfType(typeof(Save)) as Save[];
        foreach (Save saveableObject in saveableObjects) {
            // For each item, run its own specific "Serialize" function. This can differ between items thanks to ovveride methods.
            saveItems.Add(saveableObject.Serialize());
        }

        using (StreamWriter gameDataFileStream = new StreamWriter(gameDataFilePath)) {
            foreach (string item in saveItems) {
                // "Stream Writer" puts save information into the user's "My Documents" folder by default. In this case, it will create a file
                // Wherein each line will contain information for each item in the saveItems list. Thanks to "gameDataFilePath", the save information will go 
                // into that location instead.
                gameDataFileStream.WriteLine(item);
            }
        }
    }

    public void Load() {
        // When loading, we set firstPlay to false so that the "OnLevelWasLoaded" will run. Clear all items in "saveItems", then load the current scene.
        firstPlay = false;
        saveItems.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // NOTE: Current version of unity is 5.4.0f3. "OnLevelWasLoaded" is now deprecated.
    // http://answers.unity3d.com/questions/1174255/since-onlevelwasloaded-is-deprecated-in-540b15-wha.html
    void OnLevelWasLoaded(int level) {
        // If this is the first time loading the scene, don't run the following.
        if (firstPlay) return;
        // If this is not the first time, load game data, delete all old game objects present, then create the "loadable" objects.
        LoadSaveGameData();
        DestroyAllSaveableObjectsInScene();
        CreateGameObjects();
    }

    void LoadSaveGameData() {
        using (StreamReader gameDataFileStream = new StreamReader(gameDataFilePath)) {
            // Check first if the next line is empty
            while (gameDataFileStream.Peek() >= 0) {
                // save the current line to a string value, with leading and trailing white spaces trimmed.
                string line = gameDataFileStream.ReadLine().Trim();
                // So long as the line has text in it, add the string to the saveItems list.
                if (line.Length > 0) {
                    saveItems.Add(line);
                }
            }
        }
    }

    void DestroyAllSaveableObjectsInScene() {
        // Find and destroy all game objects using the "Save.cs" base class.
        Save[] saveableObjects = GameObject.FindObjectsOfType(typeof(Save)) as Save[];
        foreach (Save saveableObject in saveableObjects) {
            Destroy(saveableObject.gameObject);
        }
    }

    void CreateGameObjects() {
        foreach (string saveItem in saveItems) {
            // for each save item, read through the string and extract the "prefab" name for each one.
            string pattern = @"""prefabName"":""";
            int patternIndex = saveItem.IndexOf(pattern);
            int valueStartIndex = saveItem.IndexOf('"', patternIndex + pattern.Length - 1) + 1;
            int valueEndIndex = saveItem.IndexOf('"', valueStartIndex);
            string prefabName = saveItem.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

            // for each game item, instantiate the prefab by its given name.
            GameObject item = Instantiate(Resources.Load(prefabName)) as GameObject;
            // send the "deserialize" message to the instantiated item so it can inherit/apply its saved values (position/rotation/etc).
            item.SendMessage("Deserialize", saveItem);
        }
    }
}
