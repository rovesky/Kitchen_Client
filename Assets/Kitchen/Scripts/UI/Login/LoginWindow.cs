﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SampleClient;

public class LoginWindow : PanelBase
{

    private Button Login_Btn;
    private Button Notice_Btn;
    private Text Version_Text;

    public override void Init(params object[] args)
    {
        //Messenger<Kitchen.PocoInterfaces.LoginInfo>.AddListener(MessengerEventDef.GET_USER_DATA, LoginResponse);
        Login_Btn = transform.Find("Main/Login_Btn").GetComponent<Button>();
        Notice_Btn = transform.Find("Main/Notice_Btn").GetComponent<Button>();
        Version_Text = transform.Find("Main/Notice_Btn").GetComponent<Text>();
        SetListener();
        GameCommon.Instance.AudioManager.PlayBackground(GameCommon.Instance.AudioManager.ClipArray[0]);
    }

    private void SetListener()
    {
        if (Login_Btn)
        {
            EventTriggerListener.Get(Login_Btn.gameObject).onPointerClick = o =>
            {
                Debug.Log("登录");
                DataManager.Instance.RoomDataManager.SetMod(false);
                //LoginSuccess();
                NetworkNew.Instance.LoginRequest(SystemInfo.deviceUniqueIdentifier + 1, "22222");
            };
        }
        if (Notice_Btn)
        {
            EventTriggerListener.Get(Notice_Btn.gameObject).onPointerClick = o =>
            {
                DataManager.Instance.RoomDataManager.SetMod(true);
                SceneDataManager.nextSceneName = CommonDef.kitchen_01;//目标场景名称
                UnityEngine.SceneManagement.SceneManager.LoadScene(CommonDef.Loading);//加载进度条场景
            };
        }
    }



    public override void OnShowing()
    {
        base.OnShowing();
    }

    public override void OnClosed()
    {
        base.OnClosed();
    }

    public void LoginResponse(Kitchen.PocoInterfaces.LoginInfo info)
    {

    }

    public void LoginSuccess()
    {
        Debug.Log("Success");

        this.gameObject.SetActive(false);
        PanelManager.Instance.ClosePanel(CommonDef.LoginWindow);
        PanelManager.Instance.OpenPanel<MainWindow>(CommonDef.MainWindow);
    }

    public void LoginFail()
    {

    }

    public override void Refresh()
    {
        base.Refresh();

        Debug.Log(DataManager.Instance.UserData.Token + "<<<<<<<<<<<<<<<<<");
        if (DataManager.Instance.UserData.Token != null || DataManager.Instance.UserData.Token != "")
        {
            LoginSuccess();
        }
    }
}
