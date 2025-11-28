using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    [SerializeField] Button playPause,speedUp,speedDown;
    [SerializeField] Sprite[] sprites,pauseIcons;
    [SerializeField] Image image,pause;

    private void Start()
    {
        playPause.onClick.AddListener(PlayPause);
        speedDown.onClick.AddListener(SpeedDown);
        speedUp.onClick.AddListener(SpeedUp);
    }
    private void Update()
    {
        pause.sprite = pauseIcons[Game.main.paused ? 0 : 1];
        image.sprite = sprites[Game.main.gameSpeed];
    }

    void PlayPause()
    {
        if (Game.main.Started)
        {
            Game.main.paused = !Game.main.paused;          
        }
    }

    void SpeedUp()
    {
        int val = Mathf.Clamp(Game.main.gameSpeed + 1, 0, 4);
        Game.main.gameSpeed = val;


    }

    void SpeedDown()
    {
        int val = Mathf.Clamp(Game.main.gameSpeed - 1, 0, 4);
        Game.main.gameSpeed = val;
        image.sprite = sprites[val];
    }
}
