using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class SaveGameManager
{
    public static void LoadSave()
    {
        SaveGameData data = SaveLoad.LoadData(Game.main.saveGameName);
        Game.main.gameTime = data.gameTime;
        LoadCivs(data);
        LoadMap(data.map);
        LoadWars(data);
        LoadBattles(data);
        Game.main.saveGameName = data.SaveName;
    }
    public static void SaveSave()
    {
        SaveGameData data = new SaveGameData();
        SaveCivs(data);
        SaveMap(data.map);
        SaveWars(data);
        SaveBattles(data);
        Game.main.saveGameName = SaveLoad.SaveData(data);
    }
    static void LoadBattles(SaveGameData save)
    {
        foreach (var battle in save.ongoingBattles)
        {
            battle.LoadToBattle();
        }
        foreach (var navalBattle in save.ongoingNavalBattles)
        {
            navalBattle.LoadToBattle();
        }
    }
    static void LoadWars(SaveGameData save)
    {
        foreach (var warData in save.ongoingWars)
        {
            warData.LoadToWar();
        }
    }
    static void LoadMap(SaveGameMap map)
    {
        map.LoadTiles();
    }
    static void LoadCivs(SaveGameData save)
    {
        for(int i =0; i < save.civs.Length; i++)
        {
            LoadCiv(Game.main.civs[i],save.civs[i]);
        }
    }
    static void LoadCiv(Civilisation civ,SaveGameCiv saveGameCiv)
    {
        saveGameCiv.LoadToCiv(civ);
    }
    static void SaveBattles(SaveGameData save)
    {
        foreach (var battle in Game.main.ongoingBattles)
        {
            save.ongoingBattles.Add(new SaveGameBattle(battle));
        }
        foreach (var navalBattle in Game.main.ongoingNavalBattles)
        {
            save.ongoingNavalBattles.Add(new SaveGameNavalBattle(navalBattle));
        }
    }
    static void SaveWars(SaveGameData save)
    {
        foreach (var war in Game.main.ongoingWars)
        {
            save.ongoingWars.Add(new SaveGameWar(war));
        }
    }
    static void SaveMap(SaveGameMap map)
    {
        map.SaveTiles();
    }
    static void SaveCivs(SaveGameData save)
    {
        for (int i = 0; i < Game.main.civs.Count; i++)
        {
            SaveCiv(save.civs[i],Game.main.civs[i]);
        }
    }

    static void SaveCiv(SaveGameCiv saveGameCiv,Civilisation civ)
    {
        saveGameCiv = new SaveGameCiv();
        saveGameCiv.SaveCiv(civ);
    }
}