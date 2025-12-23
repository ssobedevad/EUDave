using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkBattle : NetworkBehaviour
{
    public Battle battle;
    public bool init = false;

    [Rpc(SendTo.NotOwner)]
    public void SyncBattleRpc(Byte[] data, NetworkObjectReference[] attackerObjects, NetworkObjectReference[] defenderObjects)
    {
        SaveGameBattle battleData = MessagePackSerializer.Deserialize<SaveGameBattle>(data);
        if (!init)
        {
            battle = new Battle(battleData);
            battle.attackerCiv = Game.main.civs[battleData.ai];
            battle.defenderCiv = Game.main.civs[battleData.di];
            battle.attackerGeneral = battleData.ag;
            battle.defenderGeneral = battleData.dg;

            List<Army> attackerArmies = new List<Army>();
            for (int i = 0; i < attackerObjects.Length; i++) 
            {
                NetworkObject netObj;
                if (attackerObjects[i].TryGet(out netObj))
                {
                    NetworkArmy netArmy = netObj.GetComponent<NetworkArmy>();
                    Army army = netArmy.army;
                    battleData.aA[i].LoadToArmy(army);
                    attackerArmies.Add(army);
                }
            }
            battle.attackingArmies.AddRange(attackerArmies);

            List<Army> defenderArmies = new List<Army>();
            for (int i = 0; i < defenderObjects.Length; i++)
            {
                NetworkObject netObj;
                if (defenderObjects[i].TryGet(out netObj))
                {
                    NetworkArmy netArmy = netObj.GetComponent<NetworkArmy>();
                    Army army = netArmy.army;
                    battleData.dA[i].LoadToArmy(army);
                    defenderArmies.Add(army);
                }
            }
            battle.defendingArmies.AddRange(defenderArmies);

            battle.attackingReserves = battleData.aR;
            battle.attackingRetreated = battleData.aT;
            battle.attackingFrontLine = battleData.aF;
            battle.attackingBackLine = battleData.aB;
            battle.attackerCount = battleData.ac;
            battle.attackerCasualties = battleData.ad;

            battle.defendingReserves = battleData.dR;
            battle.defendingRetreated = battleData.dT;
            battle.defendingFrontLine = battleData.dF;
            battle.defendingBackLine = battleData.dB;
            battle.defenderCount = battleData.dc;
            battle.defenderCasualties = battleData.dd;

            battle.attackerDiceRoll = battleData.ar;
            battle.defenderDiceRoll = battleData.dr;
            battle.WarID = battleData.wi;
            battle.active = battleData.a;
            battle.battleLength = battleData.bl;
            battle.attackPhases = battleData.ap;            
            init = true;
            battle.networkBattle = this;
        }
    }
    [Rpc(SendTo.Owner)]
    public void SendBattleDataRpc()
    {
        if (init)
        {
            SaveGameBattle battleData = new SaveGameBattle(battle);
            Byte[] data = MessagePackSerializer.Serialize(battleData);

            NetworkObjectReference[] attackerObjects = battle.attackingArmies.ConvertAll(i => new NetworkObjectReference(i.GetComponent<NetworkObject>())).ToArray();
            NetworkObjectReference[] defenderObjects = battle.defendingArmies.ConvertAll(i => new NetworkObjectReference(i.GetComponent<NetworkObject>())).ToArray();
            SyncBattleRpc(data,attackerObjects,defenderObjects);
        }
    }
    public override void OnNetworkSpawn()
    {
        SendBattleDataRpc();
    }

    [Rpc(SendTo.Server)]
    public void EndBattleRpc(bool attackerWin, bool wipe, int winnerCasualties, int loserCasualties)
    {
        if (init)
        {
            battle.DoBattleEnd(attackerWin, wipe, winnerCasualties, loserCasualties);
            EndBattleClientRpc(attackerWin, wipe, winnerCasualties, loserCasualties);
            Destroy(gameObject);
        }
    }
    [Rpc(SendTo.NotServer)]
    void EndBattleClientRpc(bool attackerWin, bool wipe, int winnerCasualties, int loserCasualties)
    {
        if (init)
        {
            battle.DoBattleEnd(attackerWin, wipe, winnerCasualties, loserCasualties);
        }
    }

    [Rpc(SendTo.Server)]
    public void BattleDiceRollRpc(int attackerRoll, int defenderRoll)
    {
        if (init)
        {
            battle.attackerDiceRoll = attackerRoll;
            battle.defenderDiceRoll = defenderRoll;
            BattleDiceRollClientRpc(attackerRoll, defenderRoll);
        }
    }
    [Rpc(SendTo.NotServer)]
    void BattleDiceRollClientRpc(int attackerRoll, int defenderRoll)
    {
        if (init)
        {
            battle.attackerDiceRoll = attackerRoll;
            battle.defenderDiceRoll = defenderRoll;
        }
    }
}
