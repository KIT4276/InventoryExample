using System.IO;
using UnityEngine;

public class SaveLoadService
{
    private const string SaveFileName = "save.json";

    private readonly string _savePath = Path.Combine(Application.persistentDataPath, SaveFileName);

    public bool HasSave()
    {
        return File.Exists(_savePath);
    }

    public void Save(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("[SaveLoadService] Save failed: data is null.");
            return;
        }

        try
        {
            string directoryPath = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"[SaveLoadService] Save failed: {exception.Message}");
        }
    }

    public GameSaveData Load()
    {
        if (!HasSave())
            return new GameSaveData();

        try
        {
            string json = File.ReadAllText(_savePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[SaveLoadService] Load failed: save file is empty.");
                return new GameSaveData();
            }

            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            if (saveData == null)
            {
                Debug.LogError("[SaveLoadService] Load failed: save data is invalid.");
                return new GameSaveData();
            }

            return saveData;
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"[SaveLoadService] Load failed: {exception.Message}");
            return new GameSaveData();
        }
    }

    public void Clear()
    {
        if (!HasSave())
            return;

        File.Delete(_savePath);
    }

    public string GetSavePath()
    {
        return _savePath;
    }
}
