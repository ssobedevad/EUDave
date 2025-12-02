using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameWar
{
    public int attackerCivID;
    public int defenderCivID;
    public SaveGameVector3Int warGoal;
    public CasusBelli casusBelli;
    public bool attackerControlsWarGoal;
    public int warGoalTimer;
    public bool active;
    public bool attackerSurrender;
    public bool defenderSurrender;
    public int lengthOfWar;
    public int WarID;

    public List<int> attackerAlliesID;
    public List<int> defenderAlliesID;

    public List<float> battleResults;
    public List<float> siegeResults;


    public SaveGameWar()
    {

    }

    public SaveGameWar(War war)
    {
        attackerCivID = war.attackerCiv.CivID;
        defenderCivID = war.defenderCiv.CivID;
        warGoal = new SaveGameVector3Int(war.warGoal);
        casusBelli = war.casusBelli;
        attackerControlsWarGoal = war.attackerControlsWarGoal;
        warGoalTimer = war.warGoalTimer;
        active = war.active;
        attackerSurrender = war.attackerSurrender;
        defenderSurrender = war.defenderSurrender;
        lengthOfWar = war.lengthOfWar;
        WarID = war.WarID;

        attackerAlliesID = war.attackerAllies.ConvertAll(i => i.CivID);
        defenderAlliesID = war.defenderAllies.ConvertAll(i => i.CivID);

        battleResults = war.battleResults;
        siegeResults = war.siegeResults;
    }

    public void LoadToWar()
    {
        Civilisation attackerCiv = Game.main.civs[attackerCivID];
        Civilisation defenderCiv = Game.main.civs[defenderCivID];
        War war = new War(attackerCiv, defenderCiv);

        war.warGoal = warGoal.GetVector3Int();
        war.casusBelli = casusBelli;
        war.attackerControlsWarGoal = attackerControlsWarGoal;
        war.warGoalTimer = warGoalTimer;
        war.active = active;
        war.attackerSurrender = attackerSurrender;
        war.defenderSurrender = defenderSurrender;
        war.lengthOfWar = lengthOfWar;
        war.WarID = WarID;

        war.attackerAllies = attackerAlliesID.ConvertAll(i => Game.main.civs[i]);
        war.defenderAllies = defenderAlliesID.ConvertAll(i => Game.main.civs[i]);

        war.battleResults = battleResults;
        war.siegeResults = siegeResults;
    }
}