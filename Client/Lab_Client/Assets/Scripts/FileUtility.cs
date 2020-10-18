using System;
using System.IO;
using UnityEngine;

public static class FileUtility
{
    private static string _basePath;

    private static string basePath
    {
        get
        {
            if (string.IsNullOrEmpty(_basePath))
            {
                _basePath = Path.Combine(Application.persistentDataPath, "Result");
            }
            return _basePath;
        }
    }

    public static void SaveAsJson<T>(T data)
    {
        try
        {
            var json = JsonUtility.ToJson(data);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"JsonConvert failed as type: {typeof(T).Name}");
                return;
            }

            var path = Path.Combine(basePath, typeof(T).Name);
            path += $"{DateTime.Now:hh-mm-ss}.json";
            using (var sr = new StreamWriter(path))
            {
                sr.Write(json);
            }
            Debug.Log($"SaveAsJson success. filepath: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveAsJson failed because of {ex}\nMessage: {ex.Message}");
        }
    }

    private static void CheckDirectory()
    {
        try
        {
            if (Directory.Exists(basePath) == false)
            {
                Directory.CreateDirectory(basePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CheckDirectory failed because of {ex}\nMessage: {ex.Message}");
        }
    }
}
