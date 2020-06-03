using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using SampleClient;

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
    public Transform BG_Img;
    public Transform Setting_Btn;
    //-----------------------------------------------
    public Transform RoomListCellRoot;
    public Transform CellItemDemo;
    private Action BgMoveEnd = delegate { };
    public override void Init(params object[] args)
    {
        InitGameObject();
        SetListener();
        SetMessager();
        DataManager.Instance.RoomDataManager.SetOwner(false);
        base.Init(args);

    }
    public void SetMessager()
    {
        Messenger<List<Kitchen.PocoInterfaces.RoomInfoP>>.AddListener(MessengerEventDef.ROOM_LIST_REFRESH, (o) =>
        {
            if (o.Count == 0)
            {
                return;
            }

            InitRoomList();
            //NetworkNew.Instance.ReadyGameRequest(o[o.Count -1].rmid);
        });
        Messenger.AddListener(MessengerEventDef.TWEENER_COMPLETE, MoveEnd);
    }

    public void RemoveMessager()
    {
        Messenger<List<Kitchen.PocoInterfaces.RoomInfoP>>.RemoveListener(MessengerEventDef.ROOM_LIST_REFRESH, (o) =>
        {
            if (o.Count == 0)
            {
                return;
            }

            InitRoomList();
        });
        Messenger.RemoveListener(MessengerEventDef.TWEENER_COMPLETE, MoveEnd);
    }

    public void InitGameObject()
    {
        Head = transform.Find("Main/Head");
        Model_Img = transform.Find("Main/Modle_Img");
        CreatRoomList_Btn = transform.Find("Main/GameSelectRoot/CreatRoomList_Btn");
        RoomListPanel = transform.Find("Main/RoomListPanel");
        GameSelectRoot = transform.Find("Main/GameSelectRoot");
        BG_Img = transform.Find("Main/RoomListBg_Img");
        Setting_Btn = transform.Find("Main/Setting_Btn");
        if (BG_Img != null)
        {
            BG_Img.gameObject.SetActive(false);
        }
        if (RoomListPanel)
        {
            RoomListGameMode_Group = RoomListPanel.Find("RoomListGameMode_Group");
            CreatRoom_Btn = RoomListPanel.Find("CreatRoom_Btn");
            EnterRoom_Btn = RoomListPanel.Find("EnterRoom_Btn");
            CellItemDemo = RoomListPanel.Find("RoomListView/Viewport/ItemDemo");
            RoomListCellRoot = RoomListPanel.Find("RoomListView/Viewport/Content");
            RoomListPanel.gameObject.SetActive(false);
        }

    }

    public void MoveEnd()
    {
        RoomListPanel.gameObject.SetActive(true);
        RoomListCellRoot.localPosition = new Vector3(0, -8000, 0);
    }
    public void SetListener()
    {
        if (CreatRoom_Btn != null)
        {
            EventTriggerListener.Get(CreatRoom_Btn.gameObject).onPointerClick = o =>
            {
                Debug.Log("创建房间");
                PanelManager.Instance.OpenPanel<RoomSettingDialog>(CommonDef.RoomSettingDialog);
            };
        }
        if (EnterRoom_Btn != null)
        {
            EventTriggerListener.Get(EnterRoom_Btn.gameObject).onPointerClick = o =>
            {

            };
        }

        if (CreatRoomList_Btn != null)
        {
            EventTriggerListener.Get(CreatRoomList_Btn.gameObject).onPointerClick = o =>
            {
                if (RoomListPanel != null)
                {
                    CleanRoomList();
                    NetworkNew.Instance.GetRoomInfoListRequest();
                    BG_Img.gameObject.SetActive(true);
                    BG_Img.GetComponent<DOTweenAnimation>().DOPlay();
                }
                if (GameSelectRoot != null)
                {
                    GameSelectRoot.gameObject.SetActive(false);
                }
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

    public void SetHeadInfo(HeadInfo info)
    {
        if (info != null)
        {
            if (Head != null)
            {
                var headIcon = Head.Find("HeadIcon_Img");
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

    public void CleanRoomList()
    {
        if (RoomListCellRoot == null)
        {
            return;
        }
        var count = RoomListCellRoot.childCount;
        for (var i = 0; i < count; i++)
        {
            Destroy(RoomListCellRoot.GetChild(i).gameObject);
        }
    }

    public void InitRoomList()
    {
        StartCoroutine("CreatCell");
    }

    IEnumerator CreatCell()
    {
        var count = DataManager.Instance.RoomDataManager.RoomInfoList.Count;
        for (var i = 0; i < count; i++)
        {
            if (CellItemDemo != null)
            {
                var obj = GameObject.Instantiate(CellItemDemo.gameObject);
                if (RoomListCellRoot)
                {
                    obj.transform.SetParent(RoomListCellRoot, false);
                    SetCellInfo(obj, DataManager.Instance.RoomDataManager.RoomInfoList[i]);
                    obj.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetCellInfo(GameObject obj, Kitchen.PocoInterfaces.RoomInfoP info)
    {
        var target = obj.transform.Find("Bg_Img");
        if (target)
        {
            target.GetComponent<Image>().sprite = GameCommon.Instance.CellBgList[info.type];
            obj.name = info.rmid;
            if (target.GetComponent<Button>())
            {
                target.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log(obj.name);
                    PanelManager.Instance.OpenPanel<EnterRoomDialog>(CommonDef.EnterRoomDialog, info);
                });
            }
        }
        target = obj.transform.Find("RoomID_Text");
        if (target)
        {
            target.GetComponent<Text>().text = string.Format("房号：{0}", info.rmid);
        }
        target = obj.transform.Find("RoomName_Text");
        if (target)
        {
            target.GetComponent<Text>().text = string.Format("名称：{0}", info.name);
        }
        target = obj.transform.Find("Lock_Img");
        if (target)
        {
            target.gameObject.SetActive(info.ciph != "");
        }


    }




    public override void OnShowing()
    {
        base.OnShowing();
    }

    public override void OnClosed()
    {
        RemoveMessager();
        base.OnClosed();

    }


}
