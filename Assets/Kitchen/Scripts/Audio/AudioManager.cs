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
        if (audioClip)
        {
            AudioSource.loop = true;
            AudioSource.clip = audioClip;
        }
        if (PlayerPrefsCtrl.Instance.GetBoolValue(CommonDef.IsQuiet))
        {

            AudioSource.Stop();
            return;
        }
        if (!GameCommon.Instance.MusicOn)
        {
            AudioSource.Stop();
            return;
        }
        if (AudioSource == null)
        {
            return;
        }
        //防止背景音乐的重复播放。
        if (gameObject.GetComponent<AudioSource>().clip == audioClip)
        {
            return;
        }
        if (audioClip)
        {
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

    public void OnVolumeChange(float curVolume)
    {

         AudioSource.volume = curVolume;

    }

    public void SetQuiet(bool isQuiet)
    {
        if (isQuiet)
        {
            AudioSource.Stop();
        }
        else
        {
            AudioSource.Play();
        }
    }


}
