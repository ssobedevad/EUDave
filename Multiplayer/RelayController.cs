using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayController
{
    
    public static async Task<string> CreateRelay()
    {
        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            Debug.Log(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData);

            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
    public static async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay With Join Code " + joinCode);
            JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData,alloc.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //private async void JoinLobbyById(string lobbyID)
    //{
    //    try
    //    {
    //        JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
    //        {
    //            Player = GetPlayer()
    //        };
    //        Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, options);
    //        joinedLobby = lobby;
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}
    //private async void QuickJoinLobby()
    //{
    //    try
    //    {
    //        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
    //        {
    //            Player = GetPlayer()
    //        };
    //        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
    //        joinedLobby = lobby;
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}

    //private async void JoinLobbyByCode(string lobbyCode)
    //{
    //    try
    //    {
    //        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
    //        {
    //            Player = GetPlayer()
    //        };
    //        Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
    //        joinedLobby = lobby;
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}
}
