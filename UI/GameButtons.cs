using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject singleplayer, multiplayer,lobbies;
    private void Awake()
    {
        buttons[0].onClick.AddListener(StartGame);        
        buttons[1].onClick.AddListener(StartGameMp);
    }
    private void Start()
    {
        if (Game.main.isMultiplayer)
        {
            singleplayer.SetActive(false);
            multiplayer.SetActive(true);
            lobbies.SetActive(true);
        }
        else
        {
            singleplayer.SetActive(true);
            multiplayer.SetActive(false);
            lobbies.SetActive(false);
        }
    }
    public void ToggleMusic()
    {
        Game.main.GetComponent<AudioSource>().enabled = !Game.main.GetComponent<AudioSource>().enabled;
    }
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void ToggleSaveGames()
    {
        UIManager.main.saveGames.SetActive(!UIManager.main.saveGames.activeSelf);
    }
    public void ToggleSettings()
    {
        UIManager.main.settings.SetActive(!UIManager.main.settings.activeSelf);
    }

    void StartGame()
    {
        Game.main.StartGame();
        UIManager.main.saveGames.SetActive(false);
        UIManager.main.settings.SetActive(false);
    }
    private void OnGUI()
    {
        if (Game.main.isMultiplayer)
        {
            string text = "Start Game";
            if (!NetworkManager.Singleton.IsServer)
            {
                text = "Waiting For Host";
            }
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
    }
    void StartGameMp()
    {
        if(Game.main.isMultiplayer && NetworkManager.Singleton.IsServer)
        {
            if(NetworkManager.Singleton.ConnectedClients.Count > 1)
            {
                Game.main.multiplayerManager.StartGameClientRpc();
            }
        }
    }
}
