#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SaveToolsMenu
{
    [MenuItem("Tools/Очистить сохранения")]
    private static void ClearSaves()
    {
        SaveLoadService saveLoadService = new();
        string savePath = saveLoadService.GetSavePath();
        saveLoadService.Clear();

        Debug.Log($"Сохранение очищено: {savePath}");
    }
}
#endif
