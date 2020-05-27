using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SampleClient;
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
        InitMessager();
        RequsetPwd();
        RequestName();
    }

    public void InitMessager()
    {
        Messenger<string>.AddListener(MessengerEventDef.REMOVE_ROOM, (o) =>
        {

        });

        Messenger<Kitchen.PocoInterfaces.RoomInfoP>.AddListener(MessengerEventDef.CUR_ROOM, (o) =>
        {
            string.Format("创建房间成功Name <<{0}", o.name);
            NetworkNew.Instance.EnterRoomRequest(o.rmid, "");
        });
    }

    public void RemoveMessager()
    {

        Messenger<Kitchen.PocoInterfaces.RoomInfoP>.RemoveListener(MessengerEventDef.CUR_ROOM, (o) =>
        {

        });
        Messenger<string>.RemoveListener(MessengerEventDef.REMOVE_ROOM, (o) =>
        {

        });
    }
    public void InitGameObject()
    {
        RoomName_Text = transform.Find("Main/RoomName_Text");
        RoomPwd_Text = transform.Find("Main/RoomPwd_Text");
        Random_Btn = transform.Find("Main/Random_Btn");
        GameMode_Group = transform.Find("Main/GameMode_Group");
        Close_Btn = transform.Find("Main/Close_Btn");
        Cnfirm_Btn = transform.Find("Main/Confirm_Btn");



        if (GameMode_Group)
        {
            GameMode_Group.gameObject.SetActive(true);
        }
    }
    public void SetListener()
    {
        if (Close_Btn != null)
        {
            EventTriggerListener.Get(Close_Btn.gameObject).onPointerClick = o =>
            {
                PanelManager.Instance.ClosePanel(CommonDef.RoomSettingDialog);
                OnClosed();
            };
        }

        if (Cnfirm_Btn != null)
        {
            EventTriggerListener.Get(Cnfirm_Btn.gameObject).onPointerClick = o =>
            {
                CreatRoom();
                Debug.Log("EnterRoom");
            };
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
            for (var i = 0; i < GameMode_Group.childCount; i++)
            {
                var target = GameMode_Group.GetChild(i).GetComponent<Toggle>();
                if (target != null)
                {
                    target.onValueChanged.AddListener((o) =>
                    {
                        var obj = target.transform.Find("Background/Image");
                        obj.gameObject.SetActive(o);
                    });
                }
            }
        }
    }

    public void CreatRoom()
    {

        NetworkNew.Instance.CreatRoomRequest(DataManager.Instance.RoomDataManager.CurRoomName, "", (byte)2);

    }

    public void RandomPwd()
    {
        RequsetPwd();
    }
    public void RandomName()
    {
        RequestName();
    }

    public void SetRootName(string roomName)
    {
        if (RoomName_Text != null)
        {
            RoomName_Text.GetComponent<Text>().text = roomName;
        }
    }
    public void RequestName()
    {
        var index = Random.Range(0, 6);
        var roomName = CommonDef.TestName[index];
        DataManager.Instance.RoomDataManager.SetCurRoomName(roomName);
        SetRootName(roomName);
    }

    public void SetPwd(int Pwd)
    {
        DataManager.Instance.RoomDataManager.SetCurPwd(Pwd.ToString());
        if (RoomPwd_Text != null)
        {
            RoomPwd_Text.GetComponent<Text>().text = Pwd.ToString();
        }

    }

    public void RequsetPwd()
    {
        var Pwd = Random.Range(1000, 9999);
        SetPwd(Pwd);

    }
    public override void OnClosed()
    {

        base.OnClosed();
        RemoveMessager();
    }

    public override void OnShowing()
    {
        base.OnShowing();
    }
    private void OnDestroy()
    {
        RemoveMessager();
    }
}
