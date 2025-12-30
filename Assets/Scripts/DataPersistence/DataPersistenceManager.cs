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
            NewGame();
        }

        LoadGame();

        if(this.gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }
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

        foreach(var dataPersistenceManager in FindAllDataPersistenceManagers())
        {
            dataPersistenceManager.SaveData(gameData);
        }
        fileHandler.Save(gameData);
    }

    public void LoadGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data to load; creating new game data.");
            NewGame();
        }

        foreach (var dataPersistenceManager in FindAllDataPersistenceManagers())
        {
            dataPersistenceManager.LoadData(gameData);
        }
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
