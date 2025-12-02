using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class SaveGameManager
{
    public static async Task LoadSave(string name)
    {
        UIManager.main.loadingScreen.currentPhase = "Pre-Load";
        SaveGameData data = await SaveLoadTestMP.LoadData(name);
        UIManager.main.loadingScreen.currentPhase = "Setup Scenario";
        Game.main.gameTime.Set(data.combat.gameTime);
        Player.myPlayer.myCivID = data.combat.civID;
        Game.main.Started = true;
        LoadCivs(data);
        LoadMap(data.map);
        LoadWars(data);
        LoadBattles(data);
        if(data.combat.civID > -1 && Game.main.civs[data.combat.civID].isActive())
        {
            CameraController.main.rb.position = Map.main.tileMapManager.tilemap.CellToWorld(Game.main.civs[data.combat.civID].capitalPos);
        }
        Game.main.saveGameName = data.SaveName;
        UIManager.main.loadingScreen.currentPhase = "Completed";
    }
    public static async Task SaveSave()
    {
        UIManager.main.loadingScreen.currentPhase = "Create Save Data";
        SaveGameData data = new SaveGameData();
        SaveCivs(data);
        SaveMap(data.map);
        SaveWars(data);
        SaveBattles(data);
        UIManager.main.loadingScreen.currentPhase = "Save Save Data";
        await SaveLoadTestMP.SaveData(data);
        if (Game.main.saveGameName.Length > 0 && Game.main.replaceSave)
        {
            SaveLoadTestMP.RemoveSave(Game.main.saveGameName);
        }
        Game.main.saveGameName = data.SaveName;
        UIManager.main.loadingScreen.currentPhase = "Completed";
    }
    static void LoadBattles(SaveGameData save)
    {
        foreach (var battle in save.combat.ongoingBattles)
        {
            battle.LoadToBattle();
        }
        foreach (var navalBattle in save.combat.ongoingNavalBattles)
        {
            navalBattle.LoadToBattle();
        }
    }
    static void LoadWars(SaveGameData save)
    {
        foreach (var warData in save.combat.ongoingWars)
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
            save.combat.ongoingBattles.Add(new SaveGameBattle(battle));
        }
        foreach (var navalBattle in Game.main.ongoingNavalBattles)
        {
            save.combat.ongoingNavalBattles.Add(new SaveGameNavalBattle(navalBattle));
        }
    }
    static void SaveWars(SaveGameData save)
    {
        foreach (var war in Game.main.ongoingWars)
        {
            save.combat.ongoingWars.Add(new SaveGameWar(war));
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
            save.civs[i] = SaveCiv(Game.main.civs[i]);
        }
    }

    static SaveGameCiv SaveCiv(Civilisation civ)
    {
        SaveGameCiv saveGameCiv = new SaveGameCiv();
        saveGameCiv.SaveCiv(civ);
        return saveGameCiv;
    }
}