using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum GameType
{
    Normal,
}
public class HeadInfo
{
    public Sprite HeadImg;
    public string text;
}
public class MainWindow : PanelBase
{

    public Transform Head;
    public Transform Model_Img;
    public Transform GameSelectRoot;
    public Transform CreatRoom_Btn;
    public Transform RoomListPanel;
    public Transform CreatRoomList_Btn;
    public Transform EnterRoom_Btn;
    public Transform RoomListGameMode_Group;
    public override void Init(params object[] args)
    {
        InitGameObject();
        SetListener();
        base.Init(args);
        
    }

    public void InitGameObject()
    {
        Head = transform.Find("Main/Head");
        Model_Img = transform.Find("Main/Model_Img");
        CreatRoomList_Btn = transform.Find("Main/GameSelectRoot/CreatRoomList_Btn");
        RoomListPanel = transform.Find("Main/RoomListPanel");
        GameSelectRoot = transform.Find("Main/GameSelectRoot");
        if (RoomListPanel)
        {
            RoomListGameMode_Group = RoomListPanel.Find("RoomListGameMode_Group");
            CreatRoom_Btn = RoomListPanel.Find("CreatRoom_Btn");
            EnterRoom_Btn = RoomListPanel.Find("EnterRoom_Btn");
            RoomListPanel.gameObject.SetActive(false);
        }
        
    }


    public void SetListener()
    {
        if (CreatRoom_Btn != null)
        {
            EventTriggerListener.Get(CreatRoom_Btn.gameObject).onPointerClick = o =>
            {
                Debug.Log("创建房间");
                PanelManager.Instance.OpenPanel<RoomSettingDialog>(GameCommon.Instance.RoomSettingDialog);
            };
        }
        if (CreatRoomList_Btn != null)
        {
            EventTriggerListener.Get(CreatRoomList_Btn.gameObject).onPointerClick = o =>
            {
                if (RoomListPanel != null)
                {
                    RoomListPanel.gameObject.SetActive(true);
                    
                }
                if (GameSelectRoot != null)
                {
                    GameSelectRoot.gameObject.SetActive(false);
                }
            };
        }

        if (EnterRoom_Btn != null)
        {
            EventTriggerListener.Get(EnterRoom_Btn.gameObject).onPointerClick = o =>
            {

            };
        }
    }

    public void SetHeadInfo(HeadInfo info)
    {
        if (info != null)
        {
            if (Head != null)
            {
                var headIcon =  Head.Find("HeadIcon_Img");
                var nameText = Head.Find("Name_Text");

                if (headIcon != null)
                {
                    headIcon.GetComponent<Image>().sprite = info.HeadImg;

                }

                if (nameText != null)
                {
                    nameText.GetComponent<Text>().text = info.text;
                }
            }
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
}
