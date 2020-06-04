using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingDialog : PanelBase
{
    public Transform Close_Btn;
    public Transform QuitGame_Btn;
    public Transform Music_Sld;
    public Transform Soundeffect_Sld;
    public Transform Quiet_Tog;


    public override void Init(params object[] args)
    {
        
        base.Init(args);
        transform.SetAsFirstSibling();
        InitGameObject();
        SetInfo();
        SetListener();
    }

    public void InitGameObject()
    {
        Close_Btn = transform.Find("Main/Close_Btn");
        QuitGame_Btn = transform.Find("Main/QuitGame_Btn");
        Music_Sld = transform.Find("Main/Music_Sld");
        Soundeffect_Sld = transform.Find("Main/Soundeffect_Sld");
        Quiet_Tog = transform.Find("Main/Quiet_Tog");
    }

    public void SetListener()
    {
        if (Close_Btn)
        {
            EventTriggerListener.Get(Close_Btn.gameObject).onPointerClick = o =>
            {
                PanelManager.Instance.ClosePanel(CommonDef.SettingDialog);
            };
        }

        if (QuitGame_Btn)
        {
            EventTriggerListener.Get(QuitGame_Btn.gameObject).onPointerClick = o =>
            {
                Application.Quit();
            };
        }
        if (Music_Sld)
        {
            Music_Sld.GetComponent<Slider>().onValueChanged.AddListener((o)=> 
            {
                GameCommon.Instance.AudioManager.OnVolumeChange(o);
            });
            EventTriggerListener.Get(Music_Sld.gameObject).onPointerUp = o =>
            {
                PlayerPrefs.SetFloat(CommonDef.MusicValue, Music_Sld.GetComponent<Slider>().value);
                PlayerPrefs.Save();
            };
        }
        if (Soundeffect_Sld)
        {
            Soundeffect_Sld.GetComponent<Slider>().onValueChanged.AddListener((o) =>
            {

            });
            EventTriggerListener.Get(Soundeffect_Sld.gameObject).onPointerUp = o =>
            {

            };
        }

        if (Quiet_Tog)
        {
            Quiet_Tog.GetComponent<Toggle>().onValueChanged.AddListener((o)=> 
            {
                GameCommon.Instance.AudioManager.SetQuiet(o);
                PlayerPrefs.SetInt(CommonDef.IsQuiet, o ? 1 : 0);
                PlayerPrefs.Save();
               
            });
        }

        
    }

    public void SetInfo()
    {
        if (Music_Sld)
        {
            Music_Sld.GetComponent<Slider>().value = PlayerPrefsCtrl.Instance.GetFloatValue(CommonDef.MusicValue);
        }
        if (Quiet_Tog)
        {
            Quiet_Tog.GetComponent<Toggle>().isOn = PlayerPrefsCtrl.Instance.GetBoolValue(CommonDef.IsQuiet);
        }
    }
    

}
