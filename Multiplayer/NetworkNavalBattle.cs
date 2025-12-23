using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkNavalBattle : NetworkBehaviour
{
    public NavalBattle battle;
    public bool init = false;

    [Rpc(SendTo.NotOwner)]
    public void SyncBattleRpc(Byte[] data, NetworkObjectReference[] attackerObjects, NetworkObjectReference[] defenderObjects)
    {
        SaveGameNavalBattle battleData = MessagePackSerializer.Deserialize<SaveGameNavalBattle>(data);
        if (!init)
        {
            battle = new NavalBattle(battleData);
            battle.attackerCiv = Game.main.civs[battleData.attackerCivID];
            battle.defenderCiv = Game.main.civs[battleData.defenderCivID];
            battle.attackerGeneral = battleData.attackerGeneral;
            battle.defenderGeneral = battleData.defenderGeneral;

            List<Fleet> attackerFleets = new List<Fleet>();
            for (int i = 0; i < attackerObjects.Length; i++)
            {
                NetworkObject netObj;
                if (attackerObjects[i].TryGet(out netObj))
                {
                    NetworkFleet netFleet = netObj.GetComponent<NetworkFleet>();
                    Fleet fleet = netFleet.fleet;
                    battleData.attackingFleets[i].LoadToFleet(fleet);
                    attackerFleets.Add(fleet);
                }
            }
            battle.attackingFleets.AddRange(attackerFleets);

            List<Fleet> defenderFleets = new List<Fleet>();
            for (int i = 0; i < defenderObjects.Length; i++)
            {
                NetworkObject netObj;
                if (defenderObjects[i].TryGet(out netObj))
                {
                    NetworkFleet netFleet = netObj.GetComponent<NetworkFleet>();
                    Fleet fleet = netFleet.fleet;
                    battleData.defendingFleets[i].LoadToFleet(fleet);
                    defenderFleets.Add(fleet);
                }
            }
            battle.defendingFleets.AddRange(defenderFleets);

            battle.attackingReserves = battleData.attackingReserves;
            battle.attackingRetreated = battleData.attackingRetreated;
            battle.attackingFrontLine = battleData.attackingFrontLine;
            battle.attackerCount = battleData.attackerCount;
            battle.attackerCasualties = battleData.attackerCasualties;

            battle.defendingReserves = battleData.defendingReserves;
            battle.defendingRetreated = battleData.defendingRetreated;
            battle.defendingFrontLine = battleData.defendingFrontLine;
            battle.defenderCount = battleData.defenderCount;
            battle.defenderCasualties = battleData.defenderCasualties;

            battle.attackerDiceRoll = battleData.attackerDiceRoll;
            battle.defenderDiceRoll = battleData.defenderDiceRoll;
            battle.WarID = battleData.WarID;
            battle.active = battleData.active;
            battle.battleLength = battleData.battleLength;
            battle.attackPhases = battleData.attackPhases;
            init = true;
            battle.networkBattle = this;
        }
    }
    [Rpc(SendTo.Owner)]
    public void SendBattleDataRpc()
    {
        if (init)
        {
            SaveGameNavalBattle battleData = new SaveGameNavalBattle(battle);
            Byte[] data = MessagePackSerializer.Serialize(battleData);

            NetworkObjectReference[] attackerObjects = battle.attackingFleets.ConvertAll(i => new NetworkObjectReference(i.GetComponent<NetworkObject>())).ToArray();
            NetworkObjectReference[] defenderObjects = battle.defendingFleets.ConvertAll(i => new NetworkObjectReference(i.GetComponent<NetworkObject>())).ToArray();
            SyncBattleRpc(data, attackerObjects, defenderObjects);
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