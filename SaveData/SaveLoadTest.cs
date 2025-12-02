using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class SaveLoadTest
{
    public static string SaveData(SaveGameData data)
    {
        string civPath = Application.persistentDataPath + "/Saves.Civs." + data.SaveName + ".json";
        string mapPath = Application.persistentDataPath + "/Saves.Map." + data.SaveName + ".json";
        string combatPath = Application.persistentDataPath + "/Saves.Combat." + data.SaveName + ".json";
        string jsonData = JsonHelper.ToJson(data.civs);
        File.WriteAllText(civPath, jsonData);        
        jsonData = JsonUtility.ToJson(data.map);
        File.WriteAllText(mapPath, jsonData);        
        jsonData = JsonUtility.ToJson(data.combat);
        File.WriteAllText(combatPath, jsonData);
        SaveSaves(data.SaveName);
        return data.SaveName;
    }
    public static SaveGameData LoadData(string saveName)
    {
        string civPath = Application.persistentDataPath + "/Saves.Civs." + saveName + ".json";
        string mapPath = Application.persistentDataPath + "/Saves.Map." + saveName + ".json";
        string combatPath = Application.persistentDataPath + "/Saves.Combat." + saveName + ".json";
        if (File.Exists(civPath) && File.Exists(mapPath) && File.Exists(combatPath))
        {
            SaveGameData data = new SaveGameData();
            string jsonString = File.ReadAllText(civPath);
            data.civs = JsonHelper.FromJson<SaveGameCiv>(jsonString);
            jsonString = File.ReadAllText(mapPath);
            data.map = JsonUtility.FromJson<SaveGameMap>(jsonString);
            jsonString = File.ReadAllText(combatPath);
            data.combat = JsonUtility.FromJson<SaveGameCombat>(jsonString);
            return data;
        }
        else
        {
            Debug.LogError("Error: Save file not found for " + saveName); return null;
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
        string civPath = Application.persistentDataPath + "/Saves.Civs." + save + ".json";
        string mapPath = Application.persistentDataPath + "/Saves.Map." + save + ".json";
        string combatPath = Application.persistentDataPath + "/Saves.Combat." + save + ".json";
        if (File.Exists(civPath) && File.Exists(mapPath) && File.Exists(combatPath))
        {
            File.Delete(civPath);
            File.Delete(mapPath);
            File.Delete(combatPath);
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