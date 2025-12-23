using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;
using static MultiplayerManager;

public class NetworkArmy : NetworkBehaviour
{
    public int civID = -1;
    public Regiment[] regiments;
    public Vector3Int pos;
    public Army army;
    public bool init = false;
    private void Awake()
    {
        army = GetComponent<Army>();
    }
    public override void OnNetworkDespawn()
    {
        if (army != null && civID > -1)
        {
            Game.main.civs[civID].armies.Remove(army);
            army.tile.armiesOnTile.Remove(army);
            army.OnDestroy();
        }
    }
    private void Start()
    {
        if (init)
        {         
            if(civID == -1 && NetworkManager.Singleton.IsServer) { Destroy(gameObject); return; }
            army.civID = civID;
            if (!Game.main.civs[civID].armies.Contains(army))
            {
                Game.main.civs[civID].armies.Add(army); 
            }
            army.regiments.AddRange(regiments);
            ArmyUIProvince uIProvince = Instantiate(UIManager.main.ArmyUIPrefab, transform.position, Quaternion.identity, UIManager.main.unitCanvas).GetComponent<ArmyUIProvince>();
            uIProvince.army = army;
            UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
        }
    }
    [Rpc(SendTo.NotOwner)]
    public void SyncArmyRpc(Byte[] data)
    {
        SaveGameArmy armyData = MessagePackSerializer.Deserialize<SaveGameArmy>(data);
        armyData.LoadToArmy(army);
        if(!init)
        {            
            civID = army.civID;
            if (!Game.main.civs[civID].armies.Contains(army))
            {
                Game.main.civs[civID].armies.Add(army);
            }
            ArmyUIProvince uIProvince = Instantiate(UIManager.main.ArmyUIPrefab, transform.position, Quaternion.identity, UIManager.main.unitCanvas).GetComponent<ArmyUIProvince>();
            uIProvince.army = army;
            UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
            init = true;
        }        
        regiments = army.regiments.ToArray();
    }
    [Rpc(SendTo.Owner)]
    public void SendArmyDataRpc()
    {
        if (init)
        {
            SaveGameArmy armyData = new SaveGameArmy(army);
            Byte[] data = MessagePackSerializer.Serialize(armyData);
            SyncArmyRpc(data);
        }
    }
    public override void OnNetworkSpawn()
    {
        pos = Map.main.tileMapManager.tilemap.WorldToCell(transform.position);        
        TileData tile = Map.main.GetTile(pos);
        tile.armiesOnTile.Add(army);
        Game.main.dayTick.AddListener(army.DayTick);
        SendArmyDataRpc();
    }
    [Rpc(SendTo.Everyone)]
    public void SetPathRpc(Vector3Int[] path)
    {
        army.path = path.ToList();
    }

    [Rpc(SendTo.NotServer)]
    public void SyncRegimentsRpc(int[] regimentStrength, float[] regimentMorale)
    {
        for(int i = 0; i < army.regiments.Count; i++)
        {
            Regiment regiment = army.regiments[i];
            regiment.size = regimentStrength[i];
            regiment.morale = regimentMorale[i];
        }
    }

    [Rpc(SendTo.Server)]
    public void EquipGeneralRpc(int generalIndex)
    {        
        Civilisation civ = Game.main.civs[army.civID];
        army.AssignGeneral(civ.generals[generalIndex]);

        EquipGeneralClientRpc(generalIndex);
    }
    [Rpc(SendTo.NotServer)]
    void EquipGeneralClientRpc(int generalIndex)
    {
        Civilisation civ = Game.main.civs[army.civID];
        army.AssignGeneral(civ.generals[generalIndex]);
    }

    void EnterExit(bool enter, Vector3Int tilePos)
    {
        TileData tile = Map.main.GetTile(tilePos);
        if (enter)
        {
            tile.armiesOnTile.Add(army);
            army.TrySiege(tile);
        }
        else
        {
            tile.armiesOnTile.Remove(army);
            army.lastPos = tilePos;
            if (!tile.hasZOC || (tile.hasZOC && !tile.HasNeighboringActiveFort(civID)))
            {
                army.lastPosNoZOC = tilePos;
            }
        }
    }
    [Rpc(SendTo.Server)]
    public void ExitEnterRpc(bool enter,Vector3Int tilePos)
    {
        EnterExit(enter, tilePos);
        ExitEnterClientRpc(enter,tilePos);
    }
    [Rpc(SendTo.NotServer)]
    void ExitEnterClientRpc(bool enter,Vector3Int tilePos)
    {
        EnterExit(enter,tilePos);
    }

    [Rpc(SendTo.Server)]
    public void ArmyActionRpc(ArmyActions actionId, int action)
    {
        ArmyAction(actionId, action);
        ArmyActionClientRpc(actionId, action);
    }
    [Rpc(SendTo.NotServer)]
    void ArmyActionClientRpc(ArmyActions actionId, int action)
    {
        ArmyAction(actionId, action);
    }
    void ArmyAction(ArmyActions actionId, int action)
    {
        switch (actionId)
        {
            case ArmyActions.Split:
                {
                    army.Split();
                    break;
                }
            case ArmyActions.Disband:
                {
                    army.Disband();
                    break;
                }
            case ArmyActions.SplitMercs:
                {
                    army.DetatchMercs();
                    break;
                }
            case ArmyActions.Consolidate:
                {
                    army.Consolidate(action == 0);
                    break;
                }
        }
    }

    public enum ArmyActions
    {
        Split,
        Disband,
        SplitMercs,
        Consolidate,
    }

    [Rpc(SendTo.Server)]
    public void CombineIntoRpc(NetworkObjectReference reference)
    {
        NetworkObject netObj;
        if (reference.TryGet(out netObj))
        {
            NetworkArmy netArmy = netObj.GetComponent<NetworkArmy>();
            Army target = netArmy.army;
            if (target.civID == army.civID && target.pos == army.pos)
            {
                target.regiments.AddRange(army.regiments);
                netArmy.SendArmyDataRpc();
                army.regiments.Clear();
                army.OnExitTile(army.tile);
                Destroy(gameObject);                
            }
        }        
    }

}
