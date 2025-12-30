using UnityEngine;
using System.IO;

// Handles file I/O for saving/loading game data. Plain class; no MonoBehaviour inheritance needed.
public class FileHandler
{
    private string dataDirPath = "";
    private string dataFileName = "gameData.json";

    public FileHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                GameData loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                return loadedData;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading game data from " + fullPath + "\n" + e);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + fullPath);
            return null;
        }
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving game data to " + fullPath + "\n" + e);
        }
    }

}
