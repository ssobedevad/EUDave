using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    private void Awake()
    {
        buttons[0].onClick.AddListener(StartGame);
        buttons[1].onClick.AddListener(ToggleSaveGames);
        buttons[2].onClick.AddListener(ToggleSettings);
        buttons[3].onClick.AddListener(QuitGame);
        buttons[4].onClick.AddListener(ToggleMusic);
    }
    void ToggleMusic()
    {
        Game.main.GetComponent<AudioSource>().enabled = !Game.main.GetComponent<AudioSource>().enabled;
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void ToggleSaveGames()
    {
        UIManager.main.saveGames.SetActive(!UIManager.main.saveGames.activeSelf);
    }
    void ToggleSettings()
    {
        UIManager.main.settings.SetActive(!UIManager.main.settings.activeSelf);
    }

    void StartGame()
    {
        Game.main.StartGame();
        UIManager.main.saveGames.SetActive(false);
        UIManager.main.settings.SetActive(false);
    }
}
