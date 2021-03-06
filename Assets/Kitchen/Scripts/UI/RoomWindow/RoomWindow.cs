﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SampleClient;
using UnityEngine.UI;
public class RoomWindow : PanelBase
{
    public GameObject Ready_Btn;
    public Transform PlayerList;
    public Transform RoomInfo;
    public Transform RightConner;
    public Transform Head;
    //head
    public Transform Name_Text;
    public Transform RoomKing;
    //room info
    public Transform RoomID_Text;
    public Transform RoomName_Text;

    public Transform Setting_Btn;
    public override void Init(params object[] args)
    {
        base.Init(args);
        InitGameObject();
        SetListener();
        InitMessenger();
        SetInfo();
    }

    public void InitGameObject()
    {
        Ready_Btn = transform.Find("Main/Ready_Btn").gameObject;
        PlayerList = transform.Find("Main/PlayerList");
        RoomInfo = transform.Find("Main/RoomInfo");
        RightConner = transform.Find("Main/RightConner");
        Setting_Btn = transform.Find("Main/Setting_Btn");
        Head = transform.Find("Main/Head");

        if (Head)
        {
            RoomKing = Head.Find("Mask/RoomKing");
            Name_Text = Head.Find("Name_Text");
        }
        if (RoomInfo)
        {
            RoomID_Text = RoomInfo.Find("RoomID_Text");
            RoomName_Text = RoomInfo.Find("RoomName_Text");
        }

    }


    public void SetInfo()
    {
        if (RoomKing)
        {
            RoomKing.gameObject.SetActive(DataManager.Instance.RoomDataManager.IsRoomOwner);
        }
        if (RoomID_Text)
        {
            RoomID_Text.GetComponent<Text>().text = string.Format("ID: {0}", DataManager.Instance.RoomDataManager.CurRoomInfo.rmid);
        }
        if (RoomName_Text)
        {
            RoomName_Text.GetComponent<Text>().text = DataManager.Instance.RoomDataManager.CurRoomInfo.name;
        }
        if (Name_Text)
        {
            Name_Text.GetComponent<Text>().text = DataManager.Instance.UserData.Token;
        }
        SetPlayerList(DataManager.Instance.RoomDataManager.EnterList);
    }

    public void SetPlayerList(List<string> nameList)
    {
        if (nameList == null)
        {
            Debug.Log("名单丢了");
            return;
        }
        if (nameList.Contains(DataManager.Instance.UserData.Token))
        {
            nameList.Remove(DataManager.Instance.UserData.Token);
        }
        var count = nameList.Count;
        Debug.Log(count + "进入列表个数 <<<<<<<<<<<<<<<<<<<<<<<<");
        if (count == 0)
        {
            Debug.Log("没其他人");
            for (var i = 0; i < PlayerList.childCount; i++)
            {
                var cell = PlayerList.GetChild(i);
                var target = cell.Find("HeadIcon_Img");
                target.gameObject.SetActive(false);
            }
            return;
        }
        if (PlayerList)
        {
            if (count > PlayerList.childCount)
            {
                Debug.Log("超员了");
            }
            for (var i = 0; i < PlayerList.childCount; i++)
            {
                var cell = PlayerList.GetChild(i);
                var target = cell.Find("HeadIcon_Img");
                target.gameObject.SetActive(i <= count - 1);


                /*
                    var cell = PlayerList.GetChild(i);
                    var target = cell.Find("RoomKing");
                    if (i == 0)
                    {
                        if (target && !DataManager.Instance.RoomDataManager.IsRoomOwner)
                        {
                            target.gameObject.SetActive(true);
                        }
                    }
                    target = cell.Find("HeadIcon_Img");
                    if (target)
                    {
                        target.gameObject.SetActive(true);
                    }
                    */
            }
        }

    }

    public void SetListener()
    {
        if (Ready_Btn)
        {
            EventTriggerListener.Get(Ready_Btn).onPointerClick = o =>
            {
                NetworkNew.Instance.ReadyGameRequest(DataManager.Instance.RoomDataManager.CurRoomId);
            };
        }

        if (Setting_Btn)
        {
            EventTriggerListener.Get(Setting_Btn.gameObject).onPointerClick = o =>
            {
                PanelManager.Instance.OpenPanel<SettingDialog>(CommonDef.SettingDialog);
            };
        }
    }

    public void InitMessenger()
    {
        Messenger<List<string>>.AddListener(MessengerEventDef.UPDATE_ENTER_PLAYER_LIST, UpdateEnterRoomInfo);
        Messenger<List<string>>.AddListener(MessengerEventDef.UPDATE_READY_PLAYER_LIST, UpdateReadyRoomInfo);
    }

    public void RemoveMessenger()
    {
        Messenger<List<string>>.RemoveListener(MessengerEventDef.UPDATE_ENTER_PLAYER_LIST, UpdateEnterRoomInfo);
        Messenger<List<string>>.RemoveListener(MessengerEventDef.UPDATE_READY_PLAYER_LIST, UpdateReadyRoomInfo);
    }

    public void UpdateEnterRoomInfo(List<string> playerList)
    {
        SetPlayerList(playerList);
    }

    public void UpdateReadyRoomInfo(List<string> PlayerList)
    {

    }


    public override void OnClosing()
    {
        base.OnClosing();
        RemoveMessenger();
    }

    private void OnDestroy()
    {

    }

}

public class PlayerCellInfo
{

}
