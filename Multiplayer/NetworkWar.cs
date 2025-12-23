using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkWar : NetworkBehaviour
{
    public War war;
    public bool init = false;

    [Rpc(SendTo.NotOwner)]
    public void SyncWarRpc(Byte[] data)
    {
        SaveGameWar warData = MessagePackSerializer.Deserialize<SaveGameWar>(data);
        if (!init)
        {
            war = warData.LoadToWar();                       
            init = true;
            war.networkWar = this;
            Game.main.ongoingWars.Add(war);
            war.attackerCiv.atWarWith.Add(war.defenderCiv.CivID);
            war.defenderCiv.atWarWith.Add(war.attackerCiv.CivID);
            foreach (var ally in war.attackerAllies)
            {
                SetupAtWarWith(ally, true);
            }
            foreach (var ally in war.defenderAllies)
            {
                SetupAtWarWith(ally, false);
            }
        }
    }
    void SetupAtWarWith(Civilisation civ,bool asAttackerAlly)
    {
        if (init)
        {
            if (asAttackerAlly)
            {
                civ.atWarWith.Add(war.defenderCiv.CivID);
                war.defenderCiv.atWarWith.Add(civ.CivID);
                foreach (var defenderAlly in war.defenderAllies)
                {
                    defenderAlly.atWarWith.Add(civ.CivID);
                    civ.atWarWith.Add(defenderAlly.CivID);
                    if (civ.allies.Contains(defenderAlly.CivID))
                    {
                        civ.BreakAlliance(defenderAlly.CivID);
                    }
                }
                war.attackerCiv.atWarTogether.Add(civ.CivID);
                civ.atWarTogether.Add(war.attackerCiv.CivID);
                foreach (var attackerAlly in war.attackerAllies)
                {
                    attackerAlly.atWarTogether.Add(civ.CivID);
                    civ.atWarTogether.Add(attackerAlly.CivID);
                }

            }
            else
            {
                civ.atWarWith.Add(war.attackerCiv.CivID);
                war.attackerCiv.atWarWith.Add(civ.CivID);
                foreach (var attackerAlly in war.attackerAllies)
                {
                    attackerAlly.atWarWith.Add(civ.CivID);
                    civ.atWarWith.Add(attackerAlly.CivID);
                    if (civ.allies.Contains(attackerAlly.CivID))
                    {
                        civ.BreakAlliance(attackerAlly.CivID);
                    }
                }
                war.defenderCiv.atWarTogether.Add(civ.CivID);
                civ.atWarTogether.Add(war.defenderCiv.CivID);
                foreach (var defenderAlly in war.defenderAllies)
                {
                    defenderAlly.atWarTogether.Add(civ.CivID);
                    civ.atWarTogether.Add(defenderAlly.CivID);
                }
            }
        }
    }
    [Rpc(SendTo.Owner)]
    public void SendWarDataRpc()
    {
        if (init)
        {
            SaveGameWar battleData = new SaveGameWar(war);
            Byte[] data = MessagePackSerializer.Serialize(battleData);        
            SyncWarRpc(data);
        }
    }
    public override void OnNetworkSpawn()
    {
        SendWarDataRpc();
    }

    [Rpc(SendTo.Server)]
    public void SyncWarScoreRpc()
    {
        if (init)
        {
            SetWarScoreRpc(war.battleResults.ToArray(), war.siegeResults.ToArray());
        }
    }

    [Rpc(SendTo.NotServer)]
    void SetWarScoreRpc(float[] battles, float[] sieges)
    {
        if (init)
        {
            war.battleResults = battles.ToList();
            war.siegeResults = sieges.ToList();
            war.UpdateWarScore();
        }
    }

    [Rpc(SendTo.Server)]
    public void JoinWarRpc(int civId, bool asAttackerAlly)
    {
        Civilisation civ = Game.main.civs[civId];
        war.JoinWar(civ, asAttackerAlly);
        JoinWarClientRpc(civId, asAttackerAlly);
    }
    [Rpc(SendTo.NotServer)]
    void JoinWarClientRpc(int civId, bool asAttackerAlly)
    {
        Civilisation civ = Game.main.civs[civId];
        war.JoinWar(civ, asAttackerAlly);
    }

    [Rpc(SendTo.Server)]
    public void EndWarRpc(int leaveCiv = -1)
    {
        //Debug.Log("End War " + war.GetName());
        EndWarClientRpc(leaveCiv);
        if (leaveCiv == -1)
        {
            war.EndWar();
            Destroy(gameObject);
        }
        else
        {
            war.LeaveWar(leaveCiv);
        }       
    }
    [Rpc(SendTo.NotServer)]
    void EndWarClientRpc(int leaveCiv = -1)
    {       
        if (leaveCiv == -1)
        {
            war.EndWar();
        }
        else
        {
            war.LeaveWar(leaveCiv);
        }
    }

}
