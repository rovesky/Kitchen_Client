using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] ClipArray = new AudioClip[3];
    public AudioSource AudioSource;
    // Start is called before the first frame update
    public void Init()
    {
        AudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame

    public void PlayBackground(AudioClip audioClip)
    {
        if (AudioSource == null)
        {
            return;
        }
        //防止背景音乐的重复播放。
        if (gameObject.GetComponent<AudioSource>().clip == audioClip)
        {
            return;
        }

        //处理全局背景音乐音量
        AudioSource.volume = 1;
        AudioSource.pitch = 1;
        if (audioClip)
        {
            AudioSource.loop = true;                      //背景音乐是循环播放的
            AudioSource.clip = audioClip;
            AudioSource.Play();
        }
        else
        {
            Debug.LogWarning("[AudioManager.cs/PlayBackground()] audioClip==null !");
        }
    }

    public void Stop()
    {
        AudioSource.Stop();
    }
}
