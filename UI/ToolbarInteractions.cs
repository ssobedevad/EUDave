using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToolbarInteractions : MonoBehaviour
{
    [SerializeField] Button menu, civList, notifs;
    [SerializeField]
    Image[] images;
    [SerializeField] GameObject civListObj,notifsBack;

    private void Start()
    {
        menu.onClick.AddListener(MainMenu);
        civList.onClick.AddListener(CivListToggle);
        notifs.onClick.AddListener(Notifs);
    }
    private void Update()
    {
        images[0].color = Color.white;
        images[1].color = civListObj.activeSelf ? Color.white : Color.gray;
        images[2].color = notifsBack.activeSelf ? Color.white : Color.gray;
    }

    async void MainMenu()
    {
        Game.main.isSaveLoad = true;
        UIManager.main.loadingScreen.display = "Saving Game File";
        UIManager.main.loadingScreen.currentPhase = "Init";
        UIManager.main.loadingScreen.gameObject.SetActive(true);
        await SaveGameManager.SaveSave();

        StartCoroutine(LoadingScreen(0, UIManager.main.loadingScreen));
    }
    IEnumerator LoadingScreen(int sceneNum,LoadingScreen screen)
    {
        screen.display = "Loading Main Menu";
        screen.currentPhase = "Init";
        screen.gameObject.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNum,LoadSceneMode.Single);

        while (async.isDone == false)
        {
            screen.currentPhase = Mathf.Round(async.progress) + "%";
            yield return null;
        }

    }

    void CivListToggle()
    {
        civListObj.SetActive(!civListObj.activeSelf);
    }

    void Notifs()
    {
        notifsBack.SetActive(!notifsBack.activeSelf);
    }
}
public static class AsyncOperationExtensions
{
    public static Task AsTask(this AsyncOperation operation)
    {
        var tcs = new TaskCompletionSource<object>();
        operation.completed += _ => { tcs.SetResult(null); };
        return tcs.Task;
    }
}
