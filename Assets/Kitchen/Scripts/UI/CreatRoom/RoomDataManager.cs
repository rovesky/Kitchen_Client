using Kitchen.PocoInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataManager
{

    public string CurRoomName { get; private set; }
    public string CurPwd { get; private set; }
    public RoomInfoP CurRoomInfo { get; private set; }
    public string CurRoomId { get; private set; }
    public List<string> EnterList { get; private set; }
    public List<string> ReadyList { get; private set; }
    public bool IsRoomOwner { get; private set; }

    public bool IsReady { get; private set; }

    public void SetCurRoomName(string name)
    {
        CurRoomName = name;
    }

    public void SetCurPwd(string pwd)
    {
        CurPwd = pwd;
    }

    public void SetCurRoomID(string roomID)
    {
        CurRoomId = roomID;

    }

    public void SetOwner(bool isOwner)
    {
        IsRoomOwner = isOwner;
    }
    public void SetIsReady(bool isReady)
    {
        IsReady = IsReady;
    }

    public List<RoomInfoP> RoomInfoList { get; private set; }

    public void GetRoomInfoList(List<RoomInfoP> roomInfoList)
    {
        RoomInfoList = roomInfoList;
        Messenger<List<RoomInfoP>>.Broadcast(MessengerEventDef.ROOM_LIST_REFRESH, RoomInfoList);
    }

    public void GetCurRoomInfo(RoomInfoP roomInfo)
    {
        CurRoomInfo = roomInfo;
        Messenger<RoomInfoP>.Broadcast(MessengerEventDef.CUR_ROOM, CurRoomInfo);
    }

    public void RemoveRoom(string id)
    {
        var count = RoomInfoList.Count;
        for (var i = 0; i < count; i++)
        {
            if (id == RoomInfoList[i].rmid)
            {
                RoomInfoList.Remove(RoomInfoList[i]);
                break;
            }
        }
    }

    public void SetReadyList(List<string> readyNameList)
    {
        if (readyNameList != null)
        {
            if(readyNameList == ReadyList)
            {
                Debug.Log("readyNameList 数据一致");
                return;
            }
            ReadyList = readyNameList;
        }
        Messenger<List<string>>.Broadcast(MessengerEventDef.UPDATE_ENTER_PLAYER_LIST, readyNameList);
    }

    public void SetEnterList(List<string> enterNameList)
    {
        if (enterNameList != null)
        {
            if (enterNameList == EnterList)
            {
                Debug.Log("enterNameList 数据一致");
                return;
            }
            EnterList = enterNameList;
            Messenger<List<string>>.Broadcast(MessengerEventDef.UPDATE_ENTER_PLAYER_LIST, enterNameList);

        }
    }


}

