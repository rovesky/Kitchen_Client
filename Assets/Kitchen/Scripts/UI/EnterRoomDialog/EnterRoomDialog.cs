using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SampleClient;

public class EnterRoomDialog : PanelBase
{

    public Transform InputPwd;
    public Transform RoomName_Text;
    public Transform Confirm_Btn;
    public Transform Close_Btn;
    //--------------------------------
    private string Pwd = "";
    private Kitchen.PocoInterfaces.RoomInfoP info;
    public override void Init(params object[] args)
    {
        base.Init(args);
        InitGameObject();
        InitListener();
        if (args != null)
        {
            info = args[0] as Kitchen.PocoInterfaces.RoomInfoP;
            if (info.ciph == "")
            {
                InputPwd.gameObject.SetActive(false);
                Pwd = "";
            }
            if (RoomName_Text)
            {
                RoomName_Text.GetComponent<Text>().text = info.name;
            }
        }

    }


    public void InitGameObject()
    {
        InputPwd = transform.Find("Main/InputPwd");
        Close_Btn = transform.Find("Close_Btn");
        RoomName_Text = transform.Find("Main/RoomName_Text");
        Confirm_Btn = transform.Find("Main/Confirm_Btn");
    }

    public void InitListener()
    {
        if (Close_Btn != null)
        {
            EventTriggerListener.Get(Close_Btn.gameObject).onPointerClick = o =>
            {
                PanelManager.Instance.ClosePanel("EnterRoomDialog");
            };
        }

        if (InputPwd)
        {
            InputPwd.GetComponent<InputField>().onEndEdit.AddListener((o) =>
            {
                Pwd = o.ToString();
                Debug.Log(o.ToString());
            });
        }

        if (Confirm_Btn)
        {
            EventTriggerListener.Get(Confirm_Btn.gameObject).onPointerClick = o =>
            {
                NetworkNew.Instance.EnterRoomRequest(info.rmid, Pwd);
                Debug.Log("Ji");
            };
        }
    }

    public void SendMsgForEnterRoom()
    {

    }


    public override void OnClosed()
    {
        base.OnClosed();
        info = new Kitchen.PocoInterfaces.RoomInfoP();
    }
}
