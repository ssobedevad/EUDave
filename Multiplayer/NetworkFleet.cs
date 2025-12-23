using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;

public class NetworkFleet : NetworkBehaviour
{
    public int civID = -1;
    public Boat[] boats;
    public Vector3Int pos;
    public Fleet fleet;
    public bool init = false;
    public override void OnNetworkDespawn()
    {
        if (fleet != null && civID > -1)
        {
            Game.main.civs[civID].fleets.Remove(fleet);
            fleet.tile.fleetsOnTile.Remove(fleet);
            fleet.OnDestroy();
        }
    }
    private void Awake()
    {
        fleet = GetComponent<Fleet>();
    }
    private void Start()
    {
        if (init)
        {
            if (civID == -1) { Destroy(gameObject); return; }
            fleet.civID = civID;
            if (!Game.main.civs[civID].fleets.Contains(fleet))
            {
                Game.main.civs[civID].fleets.Add(fleet);
            }
            fleet.boats.AddRange(boats);
            FleetUIProvince uIProvince = Instantiate(UIManager.main.FleetUIPrefab, transform.position, Quaternion.identity, UIManager.main.unitCanvas).GetComponent<FleetUIProvince>();
            uIProvince.fleet = fleet;
            UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
        }
    }
    [Rpc(SendTo.NotOwner)]
    public void SyncFleetRpc(Byte[] data)
    {
        SaveGameFleet fleetData = MessagePackSerializer.Deserialize<SaveGameFleet>(data);
        fleetData.LoadToFleet(fleet);
        if (!init)
        {
            civID = fleet.civID;
            if (!Game.main.civs[civID].fleets.Contains(fleet))
            {
                Game.main.civs[civID].fleets.Add(fleet);
            }
            FleetUIProvince uIProvince = Instantiate(UIManager.main.FleetUIPrefab, transform.position, Quaternion.identity, UIManager.main.unitCanvas).GetComponent<FleetUIProvince>();
            uIProvince.fleet = fleet;
            UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
            init = true;
        }
        boats = fleet.boats.ToArray();
    }
    [Rpc(SendTo.Owner)]
    public void SendFleetDataRpc()
    {
        if (init)
        {
            SaveGameFleet fleetData = new SaveGameFleet(fleet);
            Byte[] data = MessagePackSerializer.Serialize(fleetData);
            SyncFleetRpc(data);
        }
    }
    public override void OnNetworkSpawn()
    {
        pos = Map.main.tileMapManager.tilemap.WorldToCell(transform.position);
        TileData tile = Map.main.GetTile(pos);
        tile.fleetsOnTile.Add(fleet);
        SendFleetDataRpc();
    }
    [Rpc(SendTo.Everyone)]
    public void SetPathRpc(Vector3Int[] path)
    {
        fleet.path = path.ToList();
    }

    void EnterExit(bool enter, Vector3Int tilePos)
    {
        TileData tile = Map.main.GetTile(tilePos);
        if (enter)
        {
            tile.fleetsOnTile.Add(fleet);
            
        }
        else
        {
            tile.fleetsOnTile.Remove(fleet);
            fleet.lastPos = tilePos;
        }
    }
    [Rpc(SendTo.Server)]
    public void ExitEnterRpc(bool enter, Vector3Int tilePos)
    {
        EnterExit(enter, tilePos);
        ExitEnterClientRpc(enter, tilePos);
    }
    [Rpc(SendTo.NotServer)]
    void ExitEnterClientRpc(bool enter, Vector3Int tilePos)
    {
        EnterExit(enter, tilePos);
    }

    [Rpc(SendTo.Server)]
    public void FleetActionRpc(Vector3Int navyTile, int fleetOnTileID, FleetActions actionId, int action)
    {
        TileData tile = Map.main.GetTile(navyTile);
        if (tile.fleetsOnTile.Count <= fleetOnTileID) { return; }
        Fleet fleet = tile.fleetsOnTile[fleetOnTileID];
        FleetAction(fleet, actionId, action);
        FleetActionClientRpc(navyTile, fleetOnTileID, actionId, action);
    }
    [Rpc(SendTo.NotServer)]
    void FleetActionClientRpc(Vector3Int navyTile, int fleetOnTileID, FleetActions actionId, int action)
    {
        TileData tile = Map.main.GetTile(navyTile);
        if (tile.fleetsOnTile.Count <= fleetOnTileID) { return; }
        Fleet fleet = tile.fleetsOnTile[fleetOnTileID];
        FleetAction(fleet, actionId, action);
    }
    void FleetAction(Fleet fleet, FleetActions actionId, int action)
    {
        switch (actionId)
        {
            case FleetActions.Split:
                {
                    fleet.Split();
                    break;
                }
        }
    }

    public enum FleetActions
    {
        Split,
    }

    [Rpc(SendTo.Server)]
    public void CombineIntoRpc(NetworkObjectReference reference)
    {
        NetworkObject netObj;
        if (reference.TryGet(out netObj))
        {
            NetworkFleet netFleet = netObj.GetComponent<NetworkFleet>();
            Fleet target = netFleet.fleet;
            if (target.civID == fleet.civID && target.pos == fleet.pos)
            {
                target.boats.AddRange(fleet.boats);
                netFleet.SendFleetDataRpc();
                fleet.boats.Clear();
                fleet.OnExitTile(fleet.tile);
                Game.main.civs[fleet.civID].fleets.Remove(fleet);
                Destroy(fleet.gameObject);
            }
        }
    }
}
