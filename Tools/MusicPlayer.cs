using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource source;
    List<AudioClip> music = new List<AudioClip>();
    int lastIndex = -1;
    bool loaded;
    bool requested;
    void FixedUpdate()
    {
        if (!loaded)
        {
            if (!requested)
            {
                StartCoroutine(LoadMusic());
                requested = true;
            }
            return;
        }
        else
        {
            if (source.isActiveAndEnabled && source.isPlaying == false)
            {
                int index = Random.Range(0, music.Count);
                while (index == lastIndex)
                {
                    index = Random.Range(0, music.Count);
                }
                lastIndex = index;
                source.clip = music[index];
                source.Play();
            }
        }
    }
    IEnumerator LoadMusic()
    {
        ResourceRequest request = Resources.LoadAsync<AudioClip>("Music/song1");
        yield return new WaitUntil(()=>request.isDone);
        music.Add(request.asset as AudioClip);
        request = Resources.LoadAsync<AudioClip>("Music/song2");
        yield return new WaitUntil(() => request.isDone);
        music.Add(request.asset as AudioClip);
        request = Resources.LoadAsync<AudioClip>("Music/song3");
        yield return new WaitUntil(() => request.isDone);
        music.Add(request.asset as AudioClip);
        request = Resources.LoadAsync<AudioClip>("Music/song4");
        yield return new WaitUntil(() => request.isDone);
        music.Add(request.asset as AudioClip);
        request = Resources.LoadAsync<AudioClip>("Music/song5");
        yield return new WaitUntil(() => request.isDone);
        music.Add(request.asset as AudioClip);
        request = Resources.LoadAsync<AudioClip>("Music/song6");
        yield return new WaitUntil(() => request.isDone);
        music.Add(request.asset as AudioClip);
        loaded = true;
    }
}
