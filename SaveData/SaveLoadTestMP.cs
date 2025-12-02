using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using System;
using System.Threading;
using System.Threading.Tasks;

public static class SaveLoadTestMP
{
    public static async Task SaveData(SaveGameData data)
    {
        string gamePath = Application.persistentDataPath + "/Save" + data.SaveName;
        UIManager.main.loadingScreen.currentPhase = "Pre-Serialize";
        await Task.Run(() => SerializeData(gamePath, data));
        SaveSaves(data.SaveName);
    }
    public static void SerializeData(string gamePath, SaveGameData data)
    {
        UIManager.main.loadingScreen.currentPhase = "Serializing";
        Byte[] bytes = MessagePackSerializer.Serialize(data);
        UIManager.main.loadingScreen.currentPhase = "Write To File";
        File.WriteAllBytes(gamePath, bytes);

    }
    public static void DeserializeData(string gamePath, out SaveGameData data)
    {
        UIManager.main.loadingScreen.currentPhase = "Deserializing";
        data = MessagePackSerializer.Deserialize<SaveGameData>(File.ReadAllBytes(gamePath));
    }
    public static async Task<SaveGameData> LoadData(string saveName)
    {
        UIManager.main.loadingScreen.currentPhase = "Checking For File";
        string gamePath = Application.persistentDataPath + "/Save" + saveName;
        if (File.Exists(gamePath))
        {
            SaveGameData data = new SaveGameData();
            await Task.Run(() => DeserializeData(gamePath,out data));
            return data;
        }
        else
        {
            Debug.LogError("Error: Save file not found for " + saveName);
            return null;
        }
    }
    public static void SaveSaves(string save)
    {
        string path = Application.persistentDataPath + "/SavesData";
        FileStream stream;
        List<string> data = new List<string>();
        BinaryFormatter formatter;
        if (File.Exists(path))
        {
            formatter = new BinaryFormatter();
            stream = new FileStream(path, FileMode.Open);
            List<string> lines = formatter.Deserialize(stream) as List<string>;
            if (lines != null)
            {
                data.AddRange(lines);
            }
            stream.Close();
        }
        if (!data.Contains(save))
        {
            formatter = new BinaryFormatter();
            data.Add(save);
            stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }
    public static void RemoveSave(string save)
    {
        string path = Application.persistentDataPath + "/SavesData";
        FileStream stream;
        List<string> data = new List<string>();
        BinaryFormatter formatter;
        if (File.Exists(path))
        {
            formatter = new BinaryFormatter();
            stream = new FileStream(path, FileMode.Open);
            List<string> lines = formatter.Deserialize(stream) as List<string>;
            if (lines != null)
            {
                data.AddRange(lines);
            }
            stream.Close();
            if (data.Contains(save))
            {
                formatter = new BinaryFormatter();          
                data.Remove(save);
                stream = new FileStream(path, FileMode.Create);
                formatter.Serialize(stream, data);
                stream.Close();
            }
            else
            {
                Debug.LogError("No Saves called " + save);
                return;
            }
        }
        else 
        {
            Debug.LogError("No Saves file found in " + path);
            return; 
        }
        string gamePath = Application.persistentDataPath + "/Save" + save;
        if (File.Exists(gamePath))
        {
            File.Delete(gamePath);
        }
        else 
        {
            Debug.LogError("No save game file found by " + save);
            return;
        }

    }
    public static List<string> LoadSaves()
    {
        List<SaveGameData> saveFiles = new List<SaveGameData>();
        List<string> saves = new List<string>();
        string path = Application.persistentDataPath + "/SavesData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            saves = formatter.Deserialize(stream) as List<string>;

            stream.Close();
        }
        return saves;
    }
}