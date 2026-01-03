using UnityEngine;
using Singletons;
using System.Collections.Generic;
using System.Linq;
public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    private GameData gameData;
    public List<IDataPeristenceManager> dataPersistenceObjects;
    private FileHandler fileHandler;

    protected override void Awake()
    {
        base.Awake();

        this.fileHandler = new FileHandler(Application.persistentDataPath, "gameData.json");
        this.gameData = fileHandler.Load();
        
        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }
        
        // Only load game data into managers on first initialization
        // Don't reload if we're being created mid-game (e.g., during a save operation)
        LoadGame();
    }

    public void NewGame()
    {
        if(this.gameData == null)
        {
            this.gameData = new GameData();
        }
    }

    public void SaveGame()
    {
        if(this.gameData == null)
        {
            Debug.LogWarning("No game data to save.");
            return;
        }

        Debug.Log("DataPersistenceManager: SaveGame called");
        
        // First, gather current state from all managers
        foreach(var dataPersistenceManager in FindAllDataPersistenceManagers())
        {
            dataPersistenceManager.SaveData(gameData);
        }
        
        // Then save to file
        fileHandler.Save(gameData);
        
        Debug.Log("DataPersistenceManager: SaveGame completed");
        
        // DO NOT call LoadGame here - the data is already correct in memory
    }

    public void LoadGame()
    {
        Debug.Log("DataPersistenceManager: LoadGame called - Stack Trace: " + UnityEngine.StackTraceUtility.ExtractStackTrace());
        
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data to load; creating new game data.");
            NewGame();
        }

        foreach (var dataPersistenceManager in FindAllDataPersistenceManagers())
        {
            dataPersistenceManager.LoadData(gameData);
        }
        
        Debug.Log("DataPersistenceManager: LoadGame completed");
    }
    public void DeleteGameData()
    {
        gameData = null;
    }

    public bool HasGameData()
    {
        return gameData != null;
    }  

    private List<IDataPeristenceManager> FindAllDataPersistenceManagers()
    {
        IEnumerable<IDataPeristenceManager> dataPersistenceManagers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPeristenceManager>();
        return new List<IDataPeristenceManager>(dataPersistenceManagers);
    }

}
