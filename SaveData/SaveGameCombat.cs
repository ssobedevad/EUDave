using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameCombat
{
    public List<SaveGameBattle> ongoingBattles;
    public List<SaveGameNavalBattle> ongoingNavalBattles;
    public List<SaveGameWar> ongoingWars;
    public int civID;
    public Age gameTime;
    public SaveGameCombat()
    {        
        ongoingBattles = new List<SaveGameBattle> ();
        ongoingNavalBattles = new List<SaveGameNavalBattle>();
        ongoingWars = new List<SaveGameWar>();
    }

}