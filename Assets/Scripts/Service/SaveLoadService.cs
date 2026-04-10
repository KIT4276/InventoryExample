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

        string directoryPath = Path.GetDirectoryName(_savePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
    }

    public GameSaveData Load()
    {
        if (!HasSave())
            return new GameSaveData();

        string json = File.ReadAllText(_savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        return saveData ?? new GameSaveData();
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
