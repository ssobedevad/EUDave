using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    bool started;
    private void Awake()
    {
        started = false;
    }
    public void StartGameSingleplayer()    
    {
        if (started) { return; }
        started = true;
        PlayerPrefs.SetString("Gamemode", "Singleplayer");
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadSceneAsync(1);
    }
    public void StartGameMultiplayer()
    {
        if (started) { return; }
        started = true;
        PlayerPrefs.SetString("Gamemode", "Multiplayer");
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        if (started) { return; }
        started = true;
        Application.Quit();
    }
}
