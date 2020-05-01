using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomSettingDialog : PanelBase
{
    public Transform RoomName_Text;
    public Transform RoomPwd_Text;
    public Transform Random_Btn;
    public Transform GameMode_Group;
    public Transform Close_Btn;
    public Transform Cnfirm_Btn;
    public override void Init(params object[] args)
    {
        base.Init(args);

        InitGameObject();
        SetListener();
        RequsetPwd();
        RequestName();
    }

    public void InitGameObject()
    {
        RoomName_Text = transform.Find("Main/RoomName_Text");
        RoomPwd_Text = transform.Find("Main/RoomPwd_Text");
        Random_Btn = transform.Find("Main/Random_Btn");
        GameMode_Group = transform.Find("Main/GameMode_Group");
        Close_Btn = transform.Find("Main/Close_Btn");
        Cnfirm_Btn = transform.Find("Main/Cnfirm_Btn");

    }
    public void SetListener()
    {
        if (Close_Btn != null)
        {
            EventTriggerListener.Get(Close_Btn.gameObject).onPointerClick = o => 
            {
                PanelManager.Instance.ClosePanel(GameCommon.Instance.RoomSettingDialog);
                OnClosed();
            };
        }

        if (Cnfirm_Btn != null)
        {
            Debug.Log("EnterRoom");
        }

        if (Random_Btn != null)
        {
            EventTriggerListener.Get(Random_Btn.gameObject).onPointerClick = o =>
            {
                RandomPwd();
            };
        }

        if (RoomName_Text != null)
        {
            EventTriggerListener.Get(RoomName_Text.gameObject).onPointerClick = o =>
            {
                RandomName();
            };
        }

        if (GameMode_Group != null)
        {
            for (var i = 0; i < GameMode_Group.childCount ; i ++)
            {
                var target = GameMode_Group.GetChild(i).GetComponent<Toggle>();
                if (target != null)
                {
                    target.onValueChanged.AddListener((o) =>
                    {
                        var text = target.transform.Find("Label").GetComponent<Text>();
                        text.color = o ? Color.white : Color.black;
                    });
                }
                
               
            }
           
    }
        
    }

    public void RandomPwd()
    {
        RequsetPwd();
    }
    public void RandomName()
    {
        RequestName();
    }

    public void RequestName()
    {
        var index = Random.Range(0, 6);
        var roomName = GameCommon.Instance.TestName[index];
        if (RoomName_Text != null)
        {
            RoomName_Text.GetComponent<Text>().text = roomName;
        }
    }

    public void RequsetPwd()
    {
        var Pwd = Random.Range(1000,9999);
        if (RoomPwd_Text != null)
        {
            RoomPwd_Text.GetComponent<Text>().text = Pwd.ToString() ;
        }
    }
    public override void OnClosed()
    {
        
        base.OnClosed();
    }

    public override void OnShowing()
    {
        base.OnShowing();
    }
}
