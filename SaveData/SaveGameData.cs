using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameData
{
    public string SaveName;
    public SaveGameCiv[] civs;
    public SaveGameMap map;
    public SaveGameCombat combat;
    public SaveGameData()
    {
        civs = new SaveGameCiv[Game.main.civs.Count];
        map = new SaveGameMap();
        combat = new SaveGameCombat();
        combat.gameTime = Game.main.gameTime;
        combat.civID = Player.myPlayer.myCivID;
        SaveName = "[" + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Second + "](" + Player.myPlayer.myCivID + "){" + combat.gameTime.totalTicks() + "}";
    }
}