using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public class SaveGameData
{
    public string SaveName;
    public SaveGameCiv[] civs;
    public SaveGameMap map;
    public List<SaveGameBattle> ongoingBattles;
    public List<SaveGameNavalBattle> ongoingNavalBattles;
    public List<SaveGameWar> ongoingWars;
    public int civID;
    public Age gameTime;
    public SaveGameData()
    {
        civs = new SaveGameCiv[Game.main.civs.Count];
        map = new SaveGameMap();
        gameTime = Game.main.gameTime;
        ongoingBattles = new List<SaveGameBattle>();
        ongoingNavalBattles = new List<SaveGameNavalBattle>();
        ongoingWars = new List<SaveGameWar>();
        civID = Player.myPlayer.myCivID;
        if (Game.main.saveGameName != "")
        {
            SaveName = Game.main.saveGameName;
        }
        else
        {
            SaveName = "Save[" + Game.main.gameTime.ToString() + "]" + (Player.myPlayer.myCivID > -1 ? Player.myPlayer.myCiv.civName : "Spectator");
        }
    }

}