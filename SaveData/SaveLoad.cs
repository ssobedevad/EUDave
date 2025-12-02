using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveLoad
{
    public static string SaveData(SaveGameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Save." + data.SaveName;
        FileStream stream = new FileStream(path, FileMode.Create);             
        formatter.Serialize(stream, data.map);
        stream.Close();       
        SaveSaves(data.SaveName);
        return data.SaveName;
    }
    public static SaveGameData LoadData(string saveName)
    {
        string path = Application.persistentDataPath + "/Save." + saveName; if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            SaveGameData data = formatter.Deserialize(stream) as SaveGameData;

            stream.Close(); 
            return data;
        }
        else
        {
            Debug.LogError("Error: Save file not found in " + path); return null;
        }
    }
    public static void SaveSaves(string save)
    {
        string path = Application.persistentDataPath + "/Saves.SavesData";
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
        string path = Application.persistentDataPath + "/Saves.SavesData";
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
        path = Application.persistentDataPath + "/Save." + save; 
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else 
        {
            Debug.LogError("No save game file found in " + path);
            return;
        }

    }
    public static List<string> LoadSaves() 
    {
        List<SaveGameData> saveFiles = new List<SaveGameData>();
        List<string> saves = new List<string>();
        string path = Application.persistentDataPath + "/Saves.SavesData"; 
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