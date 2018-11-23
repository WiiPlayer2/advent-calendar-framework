using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;

public class SaveManager : MonoBehaviour
{
    private class SaveData
    {
        public Dictionary<int, bool> OpenedLocks { get; set; } = new Dictionary<int, bool>();

        public bool IsOpened(int day)
        {
            var result = false;
            if (OpenedLocks.TryGetValue(day, out result))
                return result;
            return false;
        }
    }

    [SerializeField]
    private string saveFile = "savedata.yaml";

    private IDeserializer yamlDeserializer;

    private ISerializer yamlSerializer;

    private SaveData saveData;

    private string saveFileFullName;

    private void Start()
    {
        saveFileFullName = Path.Combine(Application.persistentDataPath, saveFile);

        yamlDeserializer = new DeserializerBuilder().Build();
        yamlSerializer = new SerializerBuilder().Build();
        Load();

        foreach (var timeLock in FindObjectsOfType<TimeLock>())
        {
            if(saveData.IsOpened(timeLock.Day))
            {
                timeLock.TryOpen();
            }
            timeLock.OnStateChanged.AddListener(state => OnStateChanged(timeLock, state));
        }
    }

    private void OnStateChanged(TimeLock timeLock, TimeLock.State state)
    {
        saveData.OpenedLocks[timeLock.Day] = state == TimeLock.State.Opened;
        Save();
    }

    public void Save()
    {
        File.WriteAllText(saveFileFullName, yamlSerializer.Serialize(saveData));
        Debug.Log($"Saved data to {saveFileFullName}");
    }

    public void Load()
    {
        saveData = new SaveData();
        if (File.Exists(saveFileFullName))
        {
            try
            {
                saveData = yamlDeserializer.Deserialize<SaveData>(File.ReadAllText(saveFileFullName));
                Debug.Log($"Loaded data from {saveFileFullName}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
