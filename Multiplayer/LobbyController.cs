using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [SerializeField] GameObject lobbyPrefab,lobbyCreator;
    [SerializeField] Transform lobbyListBack;
    [SerializeField] Button createLobby,openLobbyCreator,refreshLobbies;
    [SerializeField] TMP_InputField lobbyName;
    [SerializeField] TextMeshProUGUI title;
    List<GameObject> lobbyList = new List<GameObject>();

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private string playerName;
    string KEY_START_GAME = "GameCode";
    string PLAYER_NAME = "PlayerName";

    QueryResponse lobbyQuery;
    
    async void Start()
    {
        playerName = "Dave" + UnityEngine.Random.Range(0, 100);

        InitializationOptions options = new InitializationOptions();
        options.SetProfile(playerName);
        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        createLobby.onClick.AddListener(CreateLobbyAsync);
        openLobbyCreator.onClick.AddListener(LobbyButtonClicked);
        refreshLobbies.onClick.AddListener(() => { RequestLobbyRefresh(); });

        Game.main.start.AddListener(() => Destroy(gameObject));
    }
    void LobbyButtonClicked()
    {
        if(joinedLobby == null)
        {
            lobbyCreator.SetActive(true);
        }
        else
        {
            LeaveLobby();
        }
    }
    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player()
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}                       
                    }
        };

        return player;
    }
    private async void CreateLobbyAsync()
    {
        if(lobbyName.text.Length == 0) { return; }
        try
        {
            string name = lobbyName.text;
            string joinCode = await RelayController.CreateRelay();
            if (Game.main.multiplayerManager == null)
            {
                NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(Game.main.multiplayerManagerObject);
            }
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> 
                {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }                
                
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers,options);
            Debug.Log("Created Lobby! " + name + " " + lobby.MaxPlayers);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            
            lobbyCreator.SetActive(false);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private async void JoinLobbyAtIndex(int index)
    {
        try
        {
            Lobby joining = lobbyQuery.Results[index];
            string lobbyID = joining.Id;
            Debug.Log("Joining Lobby! " + joining.Name + " " + lobbyID);
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, options);            
            joinedLobby = lobby;
            RelayController.JoinRelay(lobby.Data[KEY_START_GAME].Value);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void OnGUI()
    {
        if (!AuthenticationService.Instance.IsSignedIn) { return; }
        HandleHeartbeat();
        HandlePollForUpdates();
        if(joinedLobby == null)
        {
            title.text = "Avaliable Lobbies";
            if(lobbyQuery == null) { return; }
            int amount = lobbyQuery.Results.Count;
            while (lobbyList.Count != amount)
            {
                if (lobbyList.Count > amount)
                {
                    int lastIndex = lobbyList.Count - 1;
                    Destroy(lobbyList[lastIndex]);
                    lobbyList.RemoveAt(lastIndex);
                }
                else
                {
                    GameObject item = Instantiate(lobbyPrefab, lobbyListBack);
                    int index = lobbyList.Count;
                    lobbyList.Add(item);

                    Button[] buttons = item.GetComponentsInChildren<Button>();
                    buttons[0].onClick.AddListener(delegate { JoinLobbyAtIndex(index); });
                }
            }
            for (int i = 0; i < amount; i++)
            {
                Image[] images = lobbyList[i].GetComponentsInChildren<Image>();
                TextMeshProUGUI[] texts = lobbyList[i].GetComponentsInChildren<TextMeshProUGUI>();
                Button[] buttons = lobbyList[i].GetComponentsInChildren<Button>();
                Lobby lobby = lobbyQuery.Results[i];
                texts[0].text = lobby.Name;
                texts[1].text = lobby.Players.Count + "/" + lobby.MaxPlayers;
                texts[2].text = "Join";
                buttons[0].interactable = lobby.Players.Count < lobby.MaxPlayers;
            }
        }
        else 
        {
            title.text = joinedLobby.Name;
            int amount = joinedLobby.Players.Count;
            while (lobbyList.Count != amount)
            {
                if (lobbyList.Count > amount)
                {
                    int lastIndex = lobbyList.Count - 1;
                    Destroy(lobbyList[lastIndex]);
                    lobbyList.RemoveAt(lastIndex);
                }
                else
                {
                    GameObject item = Instantiate(lobbyPrefab, lobbyListBack);
                    int index = lobbyList.Count;
                    lobbyList.Add(item);

                    Button[] buttons = item.GetComponentsInChildren<Button>();
                    buttons[0].onClick.AddListener(delegate { JoinLobbyAtIndex(index); });
                }
            }
            for (int i = 0; i < amount; i++)
            {
                Image[] images = lobbyList[i].GetComponentsInChildren<Image>();
                TextMeshProUGUI[] texts = lobbyList[i].GetComponentsInChildren<TextMeshProUGUI>();
                Button[] buttons = lobbyList[i].GetComponentsInChildren<Button>();
                Unity.Services.Lobbies.Models.Player player = joinedLobby.Players[i];
                try 
                {                   
                    texts[0].text = player.Data[PLAYER_NAME].Value;                    
                }
                catch
                {
                    texts[0].text = "Unknown";
                }
                try
                {
                    int civID = Game.main.multiplayerManager.playerCivs.Value.values[i];
                    texts[1].text = Game.main.civs[civID].civName;
                }
                catch
                {                   
                    texts[1].text = "Spectator";
                }
                texts[2].text = "";
                buttons[0].interactable = false;
            }
        }
    }
    private async void HandleHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            hostLobby = null;
            if(Game.main.multiplayerManager != null)
            {
                Destroy(Game.main.multiplayerManager);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void HandlePollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyTimerMax = 5f;
                lobbyPollTimer = lobbyTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
        //else
        //{
        //    lobbyPollTimer -= Time.deltaTime;
        //    if (lobbyPollTimer < 0f)
        //    {
        //        float lobbyTimerMax = 5f;
        //        lobbyPollTimer = lobbyTimerMax;

        //        lobbyQuery = await ListLobbies();
        //    }
        //}
    }
    async void RequestLobbyRefresh()
    {
        lobbyQuery = await ListLobbies();
    }
    private async Task<QueryResponse> ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions()
            {
                Count = 10,
                Filters = new List<QueryFilter>()
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>()
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }

            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies Found: " + queryResponse.Results.Count);

            foreach (var lobby in queryResponse.Results)
            {
                Debug.Log("Found Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
            }
            return queryResponse;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
        
    }
}
